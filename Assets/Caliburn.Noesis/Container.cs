namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using JetBrains.Annotations;

    /// <summary>Inversion of control container. Handles dependency injection for registered types.</summary>
    [PublicAPI]
    public class Container : Container.IScope
    {
        #region Constants and Fields

        /// <summary>Map of registered types.</summary>
        private readonly Dictionary<Type, Func<ILifetime, object>> registeredTypes =
            new Dictionary<Type, Func<ILifetime, object>>();

        /// <summary>Lifetime management.</summary>
        private readonly ContainerLifetime lifetime;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Container" /> class.</summary>
        public Container()
        {
            this.lifetime = new ContainerLifetime(type => this.registeredTypes[type]);
        }

        #endregion

        #region Interfaces

        /// <summary>
        ///     Defines a type which is returned by <see cref="Container" />.Register and allows further
        ///     configuration for the registration.
        /// </summary>
        public interface IRegisteredType
        {
            /// <summary>Make the registered type a singleton.</summary>
            void AsSingleton();

            /// <summary>Make the registered type a per-scope type (single instance within a Scope).</summary>
            void PerScope();
        }

        /// <summary>Defines a scope in which per-scope objects are instantiated a single time.</summary>
        public interface IScope : IServiceProvider, IDisposable
        {
        }

        /// <summary>Defines a type adding resolution strategies to an <see cref="IScope" />.</summary>
        private interface ILifetime : IScope
        {
            object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory);

            object GetServicePerScope(Type type, Func<ILifetime, object> factory);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>Disposes any <see cref="IDisposable" /> objects owned by this container.</summary>
        public void Dispose()
        {
            this.lifetime.Dispose();
        }

        #endregion

        #region IServiceProvider Implementation

        /// <summary>Returns the object registered for the given type, if registered.</summary>
        /// <param name="type">Type as registered with the container.</param>
        /// <returns>Instance of the registered type, if registered; otherwise <see langword="null" />.</returns>
        public object GetService(Type type)
        {
            return this.registeredTypes.TryGetValue(type, out var registeredType)
                       ? registeredType(this.lifetime)
                       : null;
        }

        #endregion

        #region Public Methods

        /// <summary>Creates a new scope.</summary>
        /// <returns>The created <see cref="IScope" /> object.</returns>
        public IScope CreateScope()
        {
            return new ScopeLifetime(this.lifetime);
        }

        /// <summary>Registers a factory function which will be called to resolve the specified interface.</summary>
        /// <param name="interface">The interface to register.</param>
        /// <param name="factory">A factory function.</param>
        /// <returns>The type registration.</returns>
        public IRegisteredType Register(Type @interface, Func<object> factory)
        {
            return RegisterType(@interface, _ => factory());
        }

        /// <summary>Registers an implementation type for the specified interface.</summary>
        /// <param name="interface">The interface to register.</param>
        /// <param name="implementation">An implementing type.</param>
        /// <returns>The type registration.</returns>
        public IRegisteredType Register(Type @interface, Type implementation)
        {
            return RegisterType(@interface, FactoryFromType(implementation));
        }

        #endregion

        #region Private Methods

        /// <summary>Compiles a lambda that calls the given type's first constructor resolving arguments.</summary>
        private static Func<ILifetime, object> FactoryFromType(Type itemType)
        {
            // Get first constructor for the type.
            var constructors = itemType.GetConstructors();

            if (constructors.Length == 0)
            {
                // If no public constructor found, search for an internal constructor.
                constructors =
                    itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            }

            var constructor = constructors.First();

            // Compile constructor call as a lambda expression.
            var arg = Expression.Parameter(typeof(ILifetime));

            return (Func<ILifetime, object>)Expression.Lambda(
                                                          Expression.New(
                                                              constructor,
                                                              constructor.GetParameters()
                                                                  .Select(
                                                                      param =>
                                                                          {
                                                                              var resolve =
                                                                                  new Func<ILifetime
                                                                                      , object>(
                                                                                      lifetime =>
                                                                                          lifetime
                                                                                              .GetService(
                                                                                                  param
                                                                                                      .ParameterType));

                                                                              return Expression
                                                                                  .Convert(
                                                                                      Expression
                                                                                          .Call(
                                                                                              Expression
                                                                                                  .Constant(
                                                                                                      resolve
                                                                                                          .Target),
                                                                                              resolve
                                                                                                  .Method,
                                                                                              arg),
                                                                                      param
                                                                                          .ParameterType);
                                                                          })),
                                                          arg)
                                                      .Compile();
        }

        private IRegisteredType RegisterType(Type itemType, Func<ILifetime, object> factory)
        {
            return new RegisteredType(
                itemType,
                func => this.registeredTypes[itemType] = func,
                factory);
        }

        #endregion

        #region Nested Types

        /// <summary>Handles container lifetime management.</summary>
        private class ContainerLifetime : ObjectCache, ILifetime
        {
            public ContainerLifetime(Func<Type, Func<ILifetime, object>> getFactory)
            {
                GetFactory = getFactory;
            }

            /// <summary>Retrieves the factory function from the given type, provided by the owning container.</summary>
            public Func<Type, Func<ILifetime, object>> GetFactory { get; private set; }

            public object GetService(Type type)
            {
                return GetFactory(type)(this);
            }

            /// <summary>Singletons get cached per container.</summary>
            public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            {
                return GetCached(type, factory, this);
            }

            /// <summary>At container level, per-scope items are equivalent to singletons.</summary>
            public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            {
                return GetServiceAsSingleton(type, factory);
            }
        }

        /// <summary>Provides common caching logic for lifetimes.</summary>
        private abstract class ObjectCache
        {
            /// <summary>The instance cache.</summary>
            private readonly ConcurrentDictionary<Type, object> instanceCache =
                new ConcurrentDictionary<Type, object>();

            public void Dispose()
            {
                foreach (var obj in this.instanceCache.Values)
                {
                    (obj as IDisposable)?.Dispose();
                }
            }

            /// <summary>Gets an instance from the cache or creates and caches a new object.</summary>
            protected object GetCached(Type type,
                                       Func<ILifetime, object> factory,
                                       ILifetime lifetime)
            {
                return this.instanceCache.GetOrAdd(type, _ => factory(lifetime));
            }
        }

        /// <summary>
        ///     RegisteredType is supposed to be a short lived object tying an item to its container and
        ///     allowing users to mark it as a singleton or per-scope item.
        /// </summary>
        private class RegisteredType : IRegisteredType
        {
            private readonly Func<ILifetime, object> factory;
            private readonly Type itemType;
            private readonly Action<Func<ILifetime, object>> registerFactory;

            public RegisteredType(Type itemType,
                                  Action<Func<ILifetime, object>> registerFactory,
                                  Func<ILifetime, object> factory)
            {
                this.itemType = itemType;
                this.registerFactory = registerFactory;
                this.factory = factory;

                registerFactory(this.factory);
            }

            public void AsSingleton()
            {
                this.registerFactory(
                    lifetime => lifetime.GetServiceAsSingleton(this.itemType, this.factory));
            }

            public void PerScope()
            {
                this.registerFactory(
                    lifetime => lifetime.GetServicePerScope(this.itemType, this.factory));
            }
        }

        /// <summary>Per-scope lifetime management.</summary>
        private class ScopeLifetime : ObjectCache, ILifetime
        {
            /// <summary>Singletons come from parent container's lifetime.</summary>
            private readonly ContainerLifetime parentLifetime;

            public ScopeLifetime(ContainerLifetime parentContainer)
            {
                this.parentLifetime = parentContainer;
            }

            public object GetService(Type type)
            {
                return this.parentLifetime.GetFactory(type)(this);
            }

            /// <summary>Singleton resolution is delegated to parent lifetime.</summary>
            public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            {
                return this.parentLifetime.GetServiceAsSingleton(type, factory);
            }

            /// <summary>Per-scope objects get cached.</summary>
            public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            {
                return GetCached(type, factory, this);
            }
        }

        #endregion
    }
}