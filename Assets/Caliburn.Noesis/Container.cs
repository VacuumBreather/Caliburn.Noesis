namespace Caliburn.Noesis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>A simple IoC container.</summary>
    [PublicAPI]
    public class Container : IServiceProvider
    {
        #region Constants and Fields

        private static readonly Type DelegateType = typeof(Delegate);
        private static readonly Type EnumerableType = typeof(IEnumerable);

        private readonly List<ContainerEntry> entries;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Container" /> class.</summary>
        public Container()
        {
            this.entries = new List<ContainerEntry>();

            RegisterPerRequest(container => container);
            RegisterPerRequest<IServiceProvider>(container => container);
        }

        private Container(IEnumerable<ContainerEntry> entries)
        {
            this.entries = new List<ContainerEntry>(entries);
        }

        #endregion

        #region IServiceProvider Implementation

        object IServiceProvider.GetService(Type serviceType)
        {
            return GetInstance(serviceType);
        }

        #endregion

        #region Public Methods

        /// <summary>Creates a child container.</summary>
        /// <returns>A new child container.</returns>
        public Container CreateChildContainer()
        {
            return new Container(this.entries);
        }

        /// <summary>Gets all instances of a service type.</summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <returns>The resolved service instances.</returns>
        public TService[] GetAllInstances<TService>()
        {
            var service = typeof(TService);

            var instances = this.entries.Where(x => x.Service == service)
                                .SelectMany(e => e.Select(x => (TService)x(this)))
                                .ToArray();

            return instances;
        }

        /// <summary>Requests all instances of a service type.</summary>
        /// <param name="service">The type of the service to resolve.</param>
        /// <returns>All resolved service instances or an empty enumerable if none are found.</returns>
        public object[] GetAllInstances(Type service)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var instances = this.entries.Where(x => x.Service == service)
                                .SelectMany(e => e.Select(x => x(this)))
                                .ToArray();

            return instances;
        }

        /// <summary>Requests an instance of a service with the specified service key.</summary>
        /// <typeparam name="TService">The type of the service to resolve.</typeparam>
        /// <param name="key">(Optional) The service key.</param>
        /// <returns>The resolved service instance.</returns>
        public TService GetInstance<TService>(string key = null)
        {
            return (TService)GetInstance(typeof(TService), key);
        }

        /// <summary>Requests an instance of a service with the specified service key.</summary>
        /// <param name="service">The type of the service to resolve.</param>
        /// <param name="key">(Optional) The service key.</param>
        /// <returns>The resolved service instance.</returns>
        public object GetInstance(Type service, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var entry = this.entries.Find(x => (x.Service == service) && (x.Key == key)) ??
                        this.entries.Find(x => x.Service == service);

            if (!(entry is null))
            {
                if (entry.Count != 1)
                {
                    throw new InvalidOperationException(
                        $"Found multiple registrations for type '{service}' and key {key}.");
                }

                return entry[0](this);
            }

            if (service.IsGenericType && DelegateType.IsAssignableFrom(service))
            {
                var typeToCreate = service.GenericTypeArguments[0];
                var factoryFactoryType = typeof(FactoryFactory<>).MakeGenericType(typeToCreate);
                var factoryFactoryHost = Activator.CreateInstance(factoryFactoryType);
                var factoryFactoryMethod = factoryFactoryType.GetRuntimeMethod(
                    "Create",
                    new[]
                        {
                            typeof(Container),
                            typeof(string)
                        });

                return factoryFactoryMethod.Invoke(
                    factoryFactoryHost,
                    new object[]
                        {
                            this,
                            key
                        });
            }

            if (service.IsGenericType && EnumerableType.IsAssignableFrom(service))
            {
                if (!(key is null))
                {
                    throw new InvalidOperationException(
                        $"Requesting type '{service}' with key {key} is not supported.");
                }

                var listType = service.GenericTypeArguments[0];
                var instances = GetAllInstances(listType);
                var array = Array.CreateInstance(listType, instances.Length);

                for (var i = 0; i < array.Length; i++)
                {
                    array.SetValue(instances[i], i);
                }

                return array;
            }

            return service.IsValueType ? Activator.CreateInstance(service) : null;
        }

        /// <summary>
        ///     Determines if a handler for the service with the specified key has previously been
        ///     registered.
        /// </summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="key">(Optional) The service key.</param>
        /// <returns><c>true</c> if a handler is registered; otherwise, <c>false</c>.</returns>
        public bool IsRegistered(Type service, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            return this.entries.Any(x => (x.Service == service) && (x.Key == key));
        }

        /// <summary>
        ///     Determines if a handler for the service with the specified key has previously been
        ///     registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="key">(Optional) The service key.</param>
        /// <returns><c>true</c> if a handler is registered; otherwise, <c>false</c>.</returns>
        public bool IsRegistered<TService>(string key = null)
        {
            return IsRegistered(typeof(TService), key);
        }

        /// <summary>Registers a service instance with the container.</summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="instance">The service instance.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterInstance(Type service, object instance, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            GetOrCreateEntry(service, key).Add(_ => instance);
        }

        /// <summary>Registers a service instance with the container.</summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="instance">The service instance.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterInstance<TService>(TService instance, string key = null)
        {
            RegisterInstance(typeof(TService), instance, key);
        }

        /// <summary>Registers a service type so that a new instance is created on each request.</summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="implementation">The type of the implementation.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterPerRequest(Type service, Type implementation, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (implementation is null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            GetOrCreateEntry(service, key).Add(c => c.BuildInstance(implementation));
        }

        /// <summary>Registers a service type so that a new instance is created on each request.</summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterPerRequest<TService>(string key = null)
        {
            RegisterPerRequest<TService, TService>(key);
        }

        /// <summary>Registers a service type so that a new instance is created on each request.</summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterPerRequest<TService, TImplementation>(string key = null)
            where TImplementation : TService
        {
            RegisterPerRequest(typeof(TService), typeof(TImplementation), key);
        }

        /// <summary>Registers a custom handler for serving requests from the container.</summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterPerRequest(Type service, Func<Container, object> handler, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            GetOrCreateEntry(service, key).Add(handler);
        }

        /// <summary>Registers a custom handler for serving requests from the container.</summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterPerRequest<TService>(Func<Container, TService> handler, string key = null)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            GetOrCreateEntry(typeof(TService), key).Add(c => handler(c));
        }

        /// <summary>
        ///     Registers a service type so that an instance is created once, on first request, and the
        ///     same instance is returned to all requesters thereafter.
        /// </summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="implementation">The type of the implementation.</param>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterSingleton(Type service, Type implementation, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (implementation is null)
            {
                throw new ArgumentNullException(nameof(implementation));
            }

            object singleton = null;

            GetOrCreateEntry(service, key).Add(container => singleton ??= container.BuildInstance(implementation));
        }

        /// <summary>
        ///     Registers a service type so that an instance is created once, on first request, and the
        ///     same instance is returned to all requesters thereafter.
        /// </summary>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">(Optional) The service key.</param>
        public void RegisterSingleton<TImplementation>(string key = null)
        {
            RegisterSingleton(typeof(TImplementation), typeof(TImplementation), key);
        }

        /// <summary>
        ///     Registers the class so that an instance is created once, on first request, and the same
        ///     instance is returned to all requesters thereafter.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="key">(Optional) The key.</param>
        public void RegisterSingleton<TService, TImplementation>(string key = null)
            where TImplementation : TService
        {
            RegisterSingleton(typeof(TService), typeof(TImplementation), key);
        }

        /// <summary>
        ///     Registers the class so that an instance is created once, on first request, and the same
        ///     instance is returned to all requesters thereafter.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="key">(Optional) The key.</param>
        public void RegisterSingleton<TService>(Func<Container, TService> handler, string key = null)
        {
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            object singleton = null;
            GetOrCreateEntry(typeof(TService), key).Add(container => singleton ??= handler(container));
        }

        /// <summary>
        ///     Unregisters any handlers for the service with the specified that have previously been
        ///     registered.
        /// </summary>
        /// <param name="service">The type of the service.</param>
        /// <param name="key">(Optional) The key.</param>
        /// <returns><c>true</c> if handler is successfully removed; otherwise, <c>false</c>.</returns>
        public bool UnregisterHandler(Type service, string key = null)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            var entry = this.entries.Find(
                containerEntry => (containerEntry.Service == service) && (containerEntry.Key == key));

            return entry is { } && this.entries.Remove(entry);
        }

        /// <summary>
        ///     Unregisters any handlers for the service with the specified key that have previously been
        ///     registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if handler is successfully removed; otherwise, <c>false</c>.</returns>
        public bool UnregisterHandler<TService>(string key = null)
        {
            return UnregisterHandler(typeof(TService), key);
        }

        #endregion

        #region Protected Methods

        /// <summary>Creates an instance of the type with the specified constructor arguments.</summary>
        /// <param name="type">The type of the service.</param>
        /// <param name="args">The constructor arguments.</param>
        /// <returns>The created service instance.</returns>
        protected virtual object ActivateInstance(Type type, object[] args)
        {
            return args.Length > 0 ? Activator.CreateInstance(type, args) : Activator.CreateInstance(type);
        }

        /// <summary>
        ///     Actually does the work of creating an instance and satisfying its constructor
        ///     dependencies.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The built instance.</returns>
        protected object BuildInstance(Type type)
        {
            var constructor = type.GetConstructors()
                                  .OrderByDescending(c => c.GetParameters().Length)
                                  .FirstOrDefault(c => c.IsPublic);

            if (constructor is null)
            {
                throw new InvalidOperationException($"Type '{type}' has no public constructor.");
            }

            var args = constructor.GetParameters().Select(info => GetInstance(info.ParameterType)).ToArray();

            return ActivateInstance(type, args);
        }

        #endregion

        #region Private Methods

        private ContainerEntry GetOrCreateEntry(Type service, string key)
        {
            var entry = this.entries.Find(x => (x.Service == service) && (x.Key == key));

            if (entry is null)
            {
                entry = new ContainerEntry { Service = service, Key = key };
                this.entries.Add(entry);
            }

            return entry;
        }

        #endregion

        #region Nested Types

        private sealed class ContainerEntry : List<Func<Container, object>>
        {
            public string Key;
            public Type Service;
        }

        private sealed class FactoryFactory<T>
        {
            [UsedImplicitly]
            public Func<T> Create(Container container, string key)
            {
                return () => (T)container.GetInstance(typeof(T), key);
            }
        }

        #endregion
    }
}