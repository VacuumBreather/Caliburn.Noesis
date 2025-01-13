using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Serialization;
#if UNITY_5_5_OR_NEWER
using System.Collections.Generic;
using global::Noesis;
using UnityEngine;

#else
using System.ComponentModel;
using System.Windows;
#endif

namespace Caliburn.Noesis
{

    /// <summary>
    /// Inherit from this class to configure and run the framework.
    /// </summary>
#if UNITY_5_5_OR_NEWER
    [RequireComponent(typeof(NoesisView))]
    public abstract class BootstrapperBase<T> : MonoBehaviour, IServiceProvider
#else
    public abstract class BootstrapperBase<T> : IServiceProvider
#endif
        where T : Screen
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));

        private bool _isInitialized;

#if UNITY_5_5_OR_NEWER
        [SerializeField]
        private NoesisView noesisView;
        
        [SerializeField]
        private MonoBehaviour shellViewModel;
        
        [SerializeField]
        private List<NoesisXaml> xamls;
#endif

#if UNITY_5_5_OR_NEWER
        private MonoBehaviour ShellViewModel => shellViewModel;
        private FrameworkElement ShellView => noesisView.Content;
#else
        private PropertyChangedBase ShellViewModel { get; }
        private FrameworkElement ShellView { get; }
#endif

#if !UNITY_5_5_OR_NEWER
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapperBase{T}" /> class.
        /// </summary>
        protected BootstrapperBase(PropertyChangedBase shellViewModel, FrameworkElement shellView)
        {
            ShellViewModel = shellViewModel;
            ShellView = shellView;
            
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

        private SimpleContainer IoCContainer { get; set; }

        private ViewLocator ViewLocator { get; set; }

        /// <inheritdoc />
        /// <remarks>This method must not throw exceptions.</remarks>
        public virtual object GetService(Type serviceType)
        {
            try
            {
                return IoCContainer.GetInstance(serviceType, null);
            }
            catch (Exception serviceLookupException)
            {
                Log.Error(serviceLookupException);

                return null;
            }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>A service object of type serviceType.<br />-or-<br /><c>null</c> if there is no service object of type serviceType.</returns>
        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Shuts the UI handled by this <see cref="BootstrapperBase{T}" /> down.
        /// </summary>
        /// <remarks>This is also called when the bootstrapper is destroyed but then it is not guaranteed to finish if the shutdown process is a long running task.</remarks>
        public async UniTask ShutdownAsync()
        {
            if (!_isInitialized)
            {
                return;
            }

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
#if !UNITY_5_5_OR_NEWER
                Application.Current.Shutdown();
#endif
            }
        }

        /// <summary>
        /// Override to configure your dependency injection container.
        /// </summary>
        /// <remarks>
        ///     If you are configuring your own DI container you also need to override
        ///     <see cref="GetService" /> to let the framework use it. <br /> The following types need to be
        ///     registered here at a minimum:
        ///     <list type="bullet">
        ///         <item>An <see cref="AssemblySource" /> instance, using singleton lifetime in this scope.</item>
        ///         <item>A <see cref="ViewLocator" />, using singleton lifetime in this scope</item>
        ///         <item>
        ///             An <see cref="IServiceProvider" /> implementation, using singleton lifetime in this
        ///             scope. This is usually the bootstrapper itself.
        ///         </item>
        ///         <item>
        ///             Optionally an <see cref="ILog" />, using singleton lifetime in this scope, if you
        ///             want to provide your own to the framework
        ///         </item>
        ///         <item>All relevant views, view-models and services</item>
        ///     </list>
        /// </remarks>
        protected virtual void ConfigureIoCContainer()
        {
            IoCContainer = new SimpleContainer();

            var assemblySource = new AssemblySource();
            assemblySource.Add(GetType().Assembly);

            IoCContainer.Instance(assemblySource);
            IoCContainer.Singleton<ViewLocator, ViewLocator>();

#if UNITY_5_5_OR_NEWER
            var logger = new DebugLogger(LogManager.FrameworkLogger, this);
#else
            var logger = new DebugLogger(LogManager.FrameworkLogger);
#endif

            IoCContainer.Instance<ILog>(logger);

            foreach (var viewType in assemblySource.ViewTypes)
            {
                IoCContainer.RegisterPerRequest(viewType, null, viewType);
            }

            foreach (var type in assemblySource.ViewModelTypes)
            {
                IoCContainer.RegisterPerRequest(type, null, type);

                foreach (var @interface in type.GetInterfaces())
                {
                    IoCContainer.RegisterPerRequest(@interface, null, type);
                }
            }

            LogManager.GetLog = _ => logger;
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

#if UNITY_5_5_OR_NEWER
        private void Awake()
        {
            noesisView = GetComponent<NoesisView>();
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

        private void AddServiceProviderResource(ResourceDictionary resourceDictionary)
        {
            resourceDictionary[nameof(IServiceProvider)] = this;
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            ConfigureIoCContainer();

            Log.Info("Initializing...");

            ViewLocator = GetService<ViewLocator>();

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

            Log.Info("Resolving window manager");

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
            
            var dictionary = ShellView.Resources;

            Log.Info("Registering data templates");

            DataTemplateManager.RegisterDataTemplates(
                ViewLocator,
                dictionary,
                template => AddServiceProviderResource(template.Resources));

            AddServiceProviderResource(dictionary);

#if UNITY_5_5_OR_NEWER
            noesisView.Content.DataContext = ShellViewModel;
#else
            ShellView.DataContext = ShellViewModel;
            var window = new Window { Width = 1280, Height = 720 };
            window.Show();
            window.Closing += OnWindowClosing;
            window.Content = ShellView;
#endif

            await OnStartup();

            Log.Info("Activating {ShellViewModel}", ShellViewModel);

            if (ShellViewModel is IActivate activate)
            {
                await activate.ActivateAsync();
            }

            Log.Info("Start complete");
        }
    }
}
