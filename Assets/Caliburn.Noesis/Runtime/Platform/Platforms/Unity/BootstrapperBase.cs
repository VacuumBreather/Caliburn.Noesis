using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;

#if UNITY_5_5_OR_NEWER
using System.Runtime.CompilerServices;
using global::Noesis;
using UnityEngine;
using UnityEngine.Serialization;

#else
using System.Windows;
#endif

namespace Caliburn.Noesis
{

    /// <summary>
    /// Inherit from this class to configure and run the framework.
    /// </summary>
#if UNITY_5_5_OR_NEWER
    [RequireComponent(typeof(NoesisView))]
    public abstract class BootstrapperBase<T> : MonoBehaviour, IServiceLocator
#else
    public abstract class BootstrapperBase<T> : IServiceLocator
#endif
        where T : Screen
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));

        private bool _isInitialized;

#if UNITY_5_5_OR_NEWER
        [SerializeField]
        private List<NoesisXaml> preloadXamls;
#endif

        public Screen ShellViewModel { get; private set; }
        
        public FrameworkElement ShellView { get; private set; }

        private SimpleContainer IoCContainer { get; set; }

        private ViewLocator ViewLocator { get; set; }

#if !UNITY_5_5_OR_NEWER
        protected BootstrapperBase()
        {
            PrepareApplication();
        }
#endif

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Shuts the UI handled by this <see cref="BootstrapperBase{T}" /> down.
        /// </summary>
        /// <remarks>This is also called when the bootstrapper is destroyed, but then it is not guaranteed to finish if the shutdown process is a long running task.</remarks>
        public async UniTask ShutdownAsync()
        {
            try
            {
                Log.Info("Shutting down...");

                await OnShutdown();

                if (ShellViewModel is IDeactivate deactivate)
                {
                    await deactivate.DeactivateAsync(true);
                }

                Log.Info("Shutdown complete");
            }
            finally
            {
                _isInitialized = false;
#if UNITY_5_5_OR_NEWER
                Application.Quit();
#else
                Application.Current.Shutdown();
#endif
            }
        }

        /// <summary>
        /// Override to configure your dependency injection container.
        /// </summary>
        /// <remarks>
        ///     If you are configuring your own DI container you also need to override
        ///     <see cref="IServiceLocator" /> implementation to let the framework use it. <br /> The following types need to be
        ///     registered here at a minimum:
        ///     <list type="bullet">
        ///         <item>An <see cref="AssemblySource" /> instance, using singleton lifetime in this scope.</item>
        ///         <item>The <see cref="ViewLocator" />, using singleton lifetime in this scope</item>
        ///         <item>
        ///             A <see cref="IServiceLocator" /> implementation, using singleton lifetime in this
        ///             scope. This is usually the bootstrapper itself.
        ///         </item>
        ///         <item>All relevant views, view-models and services</item>
        ///     </list>
        /// </remarks>
        protected virtual void Configure(AssemblySource assemblySource, IEnumerable<Type> extractedTypes)
        {
            IoCContainer = new SimpleContainer();

            IoCContainer.Instance<IServiceLocator>(this);
            IoCContainer.Instance(assemblySource);
            IoCContainer.Singleton<ViewLocator, ViewLocator>();

            foreach (Type type in extractedTypes)
            {
                IoCContainer.RegisterPerRequest(type, null, type);
            }
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] { GetType().Assembly };
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        public virtual object GetInstance(Type service, string key)
        {
            return IoCContainer.GetInstance(service, key);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The located service.</returns>
        public virtual TService GetInstance<TService>(string key)
        {
            return IoCContainer.GetInstance<TService>(key);
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation
        /// </summary>
        /// <param name="service">The service to locate.</param>
        /// <returns>The located services.</returns>
        public virtual IEnumerable<object> GetAllInstances(Type service)
        {
            return IoCContainer.GetAllInstances(service);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return IoCContainer.GetAllInstances<TService>();
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        public virtual void BuildUp(object instance)
        {
            IoCContainer.BuildUp(instance);
        }

        /// <summary>
        /// Override this to modify the configuration of the <see cref="Noesis.ViewLocator" />.
        /// </summary>
        /// <param name="viewLocator">The <see cref="Noesis.ViewLocator" /> to configure.</param>
        protected virtual void ConfigureViewLocator(ViewLocator viewLocator)
        {
        }

        /// <summary>
        /// Override this to add custom logic on initialization.
        /// </summary>
        /// <returns><c>true</c> if the custom initialization logic was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool OnInitialize()
        {
            return true;
        }

        /// <summary>
        /// Override this to add custom logic on shutdown.
        /// </summary>
        protected virtual UniTask OnShutdown()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Override this to add custom logic on startup.
        /// </summary>
        protected virtual UniTask OnStartup()
        {
            return UniTask.CompletedTask;
        }

#if !UNITY_5_5_OR_NEWER
        private async void OnWindowClosing(object _, CancelEventArgs args)
        {
            if (args.Cancel)
            {
                return;
            }

            var canClose = true;

            if (ShellViewModel is IGuardClose guardClose)
            {
                canClose = await guardClose.CanCloseAsync();
            }

            args.Cancel = true;

            if (canClose)
            {
                await ShutdownAsync();
            }
        }
#endif
        private FrameworkElement EnsureShellView()
        {
#if UNITY_5_5_OR_NEWER
            return GetComponent<NoesisView>().Content;
#else
            var window = new Window { Width = 1280, Height = 720 };
            window.Show();
            window.Closing += OnWindowClosing;
            var root = new Root();
            window.Content = root;

            return root;
#endif
        }

#if UNITY_5_5_OR_NEWER
        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnDestroy()
        {
            if (_isInitialized)
            {
                Log.Warn($"Async shutdown logic not guaranteed to complete before {gameObject.name} destruction - call and await {nameof(ShutdownAsync)}() before destroying");

                await ShutdownAsync();
            }
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnDisable()
        {
            await OnDisableAsync(default);
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnEnable()
        {
            await OnEnableAsync(default);
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            await StartAsync();
        }
#endif

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            if (PlatformProvider.Current is not XamlPlatformProvider)
            {
                PlatformProvider.Current = new XamlPlatformProvider();
            }

            var assemblySourceCache = new AssemblySourceCache();
            var assemblySource = new AssemblySource();

            List<Type> extractedTypes = new();

            var baseExtractTypes = assemblySourceCache.ExtractTypes;

            assemblySourceCache.ExtractTypes = assembly =>
            {
                var baseTypes = baseExtractTypes(assembly);
                var elementTypes = assembly.GetExportedTypes()
                    .Where(t => typeof(UIElement).IsAssignableFrom(t));

                var types = baseTypes.Union(elementTypes).ToList();
                
                extractedTypes.Clear();
                extractedTypes.AddRange(types);

                return types;
            };

            assemblySource.Refresh();

            if (Execute.InDesignMode)
            {
                assemblySource.Clear();
                assemblySource.AddRange(SelectAssemblies());
            }
            else
            {
                assemblySourceCache.Install(assemblySource);
                assemblySource.AddRange(SelectAssemblies());
            }

            Configure(assemblySource, extractedTypes);

            Log.Info("Initializing...");

            ViewLocator = GetInstance<ViewLocator>(null);

            if (ViewLocator is null)
            {
                var exception = new InvalidOperationException(
                    $"View locator not found - please register {nameof(ViewLocator)} instance in DI container");
                Log.Error(exception);

                throw exception;
            }

            Log.Info("Configuring type mappings");

            ViewLocator.ConfigureTypeMappings(new TypeMappingConfiguration());
            ConfigureViewLocator(ViewLocator);

#if UNITY_5_5_OR_NEWER
            foreach (NoesisXaml xaml in preloadXamls)
            {
                xaml.Load();
            }
#endif

            var wasOnInitializeSuccessful = OnInitialize();

            if (!wasOnInitializeSuccessful)
            {
                var exception = new InvalidOperationException("Initialization failed");
                Log.Error(exception);

                throw exception;
            }

            _isInitialized = true;
            Log.Info("Initialization complete");
        }

#if !UNITY_5_5_OR_NEWER
        /// <summary>
        /// Provides an opportunity to hook into the application object.
        /// </summary>
        protected virtual void PrepareApplication()
        {
            Application.Current.Startup += async (_, __) =>
            {
                await OnEnableAsync(default);
                await StartAsync();
            };
            Application.Current.Activated += async (_, __) =>
            {
                await OnEnableAsync(default);
            };
            Application.Current.Deactivated += async (_, __) =>
            {
                await OnDisableAsync(default);
            };
        }
#endif

        private async UniTask OnDisableAsync(CancellationToken cancellationToken)
        {
            if (ShellViewModel is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false, cancellationToken);
            }
        }

        private async UniTask OnEnableAsync(CancellationToken cancellationToken)
        {
            if (ShellViewModel is IActivate activate)
            {
                await activate.ActivateAsync(cancellationToken);
            }
        }

        private async UniTask StartAsync()
        {
            Initialize();

            if (!_isInitialized)
            {
                return;
            }

            Log.Info("Starting...");

            ShellView = EnsureShellView();
            ShellView.Resources[nameof(IServiceLocator)] = this;
            
            ShellViewModel = GetInstance<T>(null);
            ShellView.DataContext = this;

            await OnStartup();

            Log.Info($"Activating {ShellViewModel}");

            if (ShellViewModel is IActivate activate)
            {
                await activate.ActivateAsync();
            }

            Log.Info("Start complete");
        }
    }
}
