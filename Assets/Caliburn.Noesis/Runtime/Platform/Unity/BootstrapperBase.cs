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
    public abstract class BootstrapperBase : MonoBehaviour, IServiceLocator
#else
    public abstract class BootstrapperBase : IServiceLocator
#endif
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));

        private bool _isInitialized;
        private IEnumerable<Type> _extractedTypes;

#if UNITY_5_5_OR_NEWER
        [SerializeField]
        private List<NoesisXaml> preloadXamls;
#endif

        public object ShellViewModel { get; private set; }
        
        public FrameworkElement ShellView { get; private set; }

        protected bool EnableDisableWithThis { get; set; }

        protected ViewLocator ViewLocator { get; private set; }

        private SimpleContainer IoCContainer { get; set; }

#if !UNITY_5_5_OR_NEWER
        protected BootstrapperBase()
        {
            Initialize();
        }
#endif

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Shuts the UI handled by this <see cref="BootstrapperBase" /> down.
        /// </summary>
        /// <remarks>This is also called when the bootstrapper is destroyed, but then it is not guaranteed to finish if the shutdown process is a long running task.</remarks>
        public async UniTask ShutdownAsync()
        {
            try
            {
                Log.Info("Shutting down...");

                await OnShutdownAsync();

                if (ShellViewModel is IDeactivate deactivate)
                {
                    await deactivate.DeactivateAsync(true);
                }
            
                await GetInstance<IDialogService>().DeactivateAsync(true);

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
        ///     registered here:
        ///     <list type="bullet">
        ///         <item>A <see cref="IDialogService" /> implementation.</item>
        ///         <item>A <see cref="IEventAggregator" /> implementation.</item>
        ///         <item>All relevant views, view-models and services</item>
        ///     </list>
        /// </remarks>
        protected virtual void Configure()
        {
            IoCContainer = new SimpleContainer();

            IoCContainer.Singleton<ViewLocator, ViewLocator>();
            IoCContainer.Singleton<IDialogService, DialogConductor>();

            foreach (Type type in _extractedTypes)
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

        /// <inheritdoc />
        /// <remarks>Override this to provide an IoC specific implementation</remarks>
        public virtual object GetInstance(Type service, string key = null)
        {
            return IoCContainer.GetInstance(service, key);
        }

        /// <inheritdoc />
        public TService GetInstance<TService>(string key = null)
        {
            return (TService)GetInstance(typeof(TService), key);
        }

        /// <inheritdoc />
        /// <remarks>Override this to provide an IoC specific implementation</remarks>
        public virtual IEnumerable<object> GetAllInstances(Type service)
        {
            return IoCContainer.GetAllInstances(service);
        }

        /// <inheritdoc />
        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return GetAllInstances(typeof(TService)).Cast<TService>();
        }

        /// <inheritdoc />
        /// <remarks>Override this to provide an IoC specific implementation</remarks>
        public virtual void BuildUp(object instance)
        {
            IoCContainer.BuildUp(instance);
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
        protected virtual UniTask OnShutdownAsync()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Override this to add custom logic on startup.
        /// </summary>
        protected virtual UniTask OnStartupAsync()
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
            var root = new RootView();
            window.Content = root;

            return root;
#endif
        }

#if UNITY_5_5_OR_NEWER
        private void Awake()
        {
            Initialize();
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnEnable()
        {
            await OnEnableAsync();
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            await StartAsync();
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnDisable()
        {
            await OnDisableAsync();
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid OnDestroy()
        {
            if (_isInitialized)
            {
                Log.Warn($"Async shutdown logic not guaranteed to complete before {gameObject.name} destruction - call and await {nameof(ShutdownAsync)}() before destroying");

                await ShutdownAsync();
            }
        }
#endif

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            Log.Info("Initializing...");

            if (PlatformProvider.Current is not XamlPlatformProvider)
            {
                PlatformProvider.Current = new XamlPlatformProvider();
            }

            var assemblySourceCache = new AssemblySourceCache();
            var assemblySource = new AssemblySource();

            var baseExtractTypes = assemblySourceCache.ExtractTypes;

            assemblySourceCache.ExtractTypes = assembly =>
            {
                var baseTypes = baseExtractTypes(assembly);
                var elementTypes = assembly.GetExportedTypes()
                    .Where(t => typeof(UIElement).IsAssignableFrom(t));

                return _extractedTypes = baseTypes.Union(elementTypes).ToList();
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

#if !UNITY_5_5_OR_NEWER
            PrepareApplication();
#endif

            ViewLocator = new ViewLocator(assemblySource, this);
            Configure();

#if UNITY_5_5_OR_NEWER
            Log.Info("Preloading xamls");

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
        protected void PrepareApplication()
        {
            Application.Current.Startup += async (_, __) =>
            {
                await OnEnableAsync();
                await StartAsync();
            };
            Application.Current.Activated += async (_, __) =>
            {
                await OnEnableAsync();
            };
            Application.Current.Deactivated += async (_, __) =>
            {
                await OnDisableAsync();
            };
        }
#endif

        private async UniTask OnEnableAsync()
        {
            if (!EnableDisableWithThis)
            {
                return;
            }
            
            if (ShellViewModel is IActivate activate)
            {
                await activate.ActivateAsync();
            }
        }

        private async UniTask StartAsync()
        {
            if (!_isInitialized)
            {
                return;
            }

            Log.Info("Starting...");

            ShellView = EnsureShellView();
            ShellView.SetValue(AttachedProperties.ServiceLocatorProperty, this);
            ShellView.SetValue(AttachedProperties.ViewLocatorProperty, ViewLocator);

            await OnStartupAsync();
            
            if (GetInstance<IDialogService>() is { } dialogService)
            {
                await dialogService.ActivateAsync();
            }

            Log.Info("Start complete");
        }

        private async UniTask OnDisableAsync()
        {
            if (!EnableDisableWithThis)
            {
                return;
            }

            if (ShellViewModel is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false);
            }
        }

        /// <summary>
        /// Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <param name="viewModelType">The view model type.</param>
        /// <param name="settings">The optional window settings.</param>
        protected async UniTask DisplayRootViewForAsync(Type viewModelType, IDictionary<string, object> settings = null)
        {
            ShellViewModel = GetInstance(viewModelType);
            ShellView.DataContext = this;

            Log.Info($"Activating {ShellViewModel}");

            if (ShellViewModel is IActivate activate)
            {
                await activate.ActivateAsync();
            }
        }


        /// <summary>
        /// Locates the view model, locates the associate view, binds them and shows it as the root view.
        /// </summary>
        /// <typeparam name="TViewModel">The view model type.</typeparam>
        /// <param name="settings">The optional window settings.</param>
        protected UniTask DisplayRootViewForAsync<TViewModel>(IDictionary<string, object> settings = null)
        {
            return DisplayRootViewForAsync(typeof(TViewModel), settings);
        }
    }
}
