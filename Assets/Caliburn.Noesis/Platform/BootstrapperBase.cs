namespace Caliburn.Noesis
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
#if UNITY_5_5_OR_NEWER
    using System.Collections.Generic;
    using global::Noesis;
    using UnityEngine;
#else
    using System.ComponentModel;
    using System.Windows;
#endif

    #endregion

    /// <summary>Inherit from this class to configure and run the framework.</summary>
    [PublicAPI]
#if UNITY_5_5_OR_NEWER
    [RequireComponent(typeof(NoesisView))]
    public abstract class BootstrapperBase<T> : MonoBehaviour, IServiceProvider
#else
    public abstract class BootstrapperBase<T> : IServiceProvider
#endif
        where T : Screen
    {
        #region Constants and Fields

        private bool isInitialized;

#if UNITY_5_5_OR_NEWER
        private NoesisView noesisView;
#endif

        private UniTaskCompletionSource onStartCompletion;
        private UniTaskCompletionSource onEnableCompletion;
        private UniTaskCompletionSource onDisableCompletion;
        private UniTaskCompletionSource shutdownCompletion;

        private CancellationTokenSource onEnableCancellation;
        private CancellationTokenSource onDisableCancellation;

#if UNITY_5_5_OR_NEWER

        #region Serialized Fields

        [SerializeField]
        [UsedImplicitly]
        private List<NoesisXaml> xamls;
#endif

        #endregion

#if !UNITY_5_5_OR_NEWER

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BootstrapperBase{T}" /> class.</summary>
        protected BootstrapperBase()
        {
            Execute.Dispatcher = Dispatcher.CurrentDispatcher;
            Application.Current.Startup += (_, __) => Execute.OnUIThreadAsync(Start).Forget();
            Application.Current.Activated += (_, __) =>  Execute.OnUIThreadAsync(OnEnable).Forget();
            Application.Current.Deactivated +=
                (_, __) =>  Execute.OnUIThreadAsync(OnDisable).Forget();
        }

        #endregion

#endif

        #region Protected Properties

        /// <summary>Gets or sets the <see cref="Microsoft.Extensions.Logging.ILogger" /> for this instance.</summary>
        protected ILogger Logger => LogManager.FrameworkLogger;

        #endregion

        #region Private Properties

        private Container IoCContainer { get; set; }

        private IWindowManager WindowManager { get; set; }

        private ViewLocator ViewLocator { get; set; }

        #endregion

        #region IServiceProvider Implementation

        /// <inheritdoc />
        /// <remarks>This method must not throw exceptions.</remarks>
        public virtual object GetService(Type serviceType)
        {
            try
            {
                return IoCContainer.GetInstance(serviceType);
            }
            catch (Exception serviceLookupException)
            {
                Logger.LogError(serviceLookupException, "Failed to find service {ServiceType}", serviceType);

                return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Gets the service object of the specified type.</summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <returns>
        ///     A service object of type serviceType.<br />-or-<br /><c>null</c> if there is no service
        ///     object of type serviceType.
        /// </returns>
        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return GetType().Name;
        }

        /// <summary>Shuts the UI handled by this <see cref="BootstrapperBase{T}" /> down.</summary>
        /// <remarks>
        ///     This is also called when the bootstrapper is destroyed but then it is not guaranteed to
        ///     finish if the shutdown process is a long running task.
        /// </remarks>
        public async UniTask ShutdownAsync()
        {
            using var _ = Logger.GetMethodTracer();

            if (!this.isInitialized || (this.shutdownCompletion != null))
            {
                return;
            }

            try
            {
                Logger.LogInformation("Shutting down...");

                using var guard = TaskCompletion.CreateGuard(out this.shutdownCompletion);

                await (this.onStartCompletion?.Task ?? UniTask.CompletedTask);
                await (this.onDisableCompletion?.Task ?? UniTask.CompletedTask);

                this.onEnableCancellation?.Cancel();

                await OnShutdown();

                if (WindowManager is IDeactivate deactivate)
                {
                    await deactivate.DeactivateAsync(true);
                }

                Logger.LogInformation("Shutdown complete");
            }
            finally
            {
                this.isInitialized = false;
#if !UNITY_5_5_OR_NEWER
                Application.Current.Shutdown();
#endif
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>Override to configure your dependency injection container.</summary>
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
        ///             An <see cref="IWindowManager" /> implementation, using singleton lifetime in this
        ///             scope
        ///         </item>
        ///         <item>
        ///             Optionally an <see cref="ILogger" />, using singleton lifetime in this scope, if you
        ///             want to provide your own to the framework
        ///         </item>
        ///         <item>All relevant views, view-models and services</item>
        ///     </list>
        /// </remarks>
        protected virtual void ConfigureIoCContainer()
        {
            IoCContainer = new Container();

            var assemblySource = new AssemblySource();
            assemblySource.Add(GetType().Assembly);

            IoCContainer.RegisterInstance(assemblySource);
            IoCContainer.RegisterSingleton<ViewLocator, ViewLocator>();
            IoCContainer.RegisterSingleton<IWindowManager, ShellViewModel>();

#if UNITY_5_5_OR_NEWER
            var logger = new DebugLogger(LogManager.FrameworkCategoryName, this);
#else
            var logger = new DebugLogger(LogManager.FrameworkCategoryName);
#endif

            IoCContainer.RegisterInstance(typeof(ILogger), logger);

            foreach (var viewType in assemblySource.ViewTypes)
            {
                IoCContainer.RegisterPerRequest(viewType, viewType);
            }

            foreach (var type in assemblySource.ViewModelTypes)
            {
                IoCContainer.RegisterPerRequest(type, type);

                foreach (var @interface in type.GetInterfaces())
                {
                    IoCContainer.RegisterPerRequest(@interface, type);
                }
            }

            LogManager.FrameworkLogger = logger;
        }

        /// <summary>Override this to modify the configuration of the <see cref="Noesis.ViewLocator" />.</summary>
        /// <param name="viewLocator">The <see cref="Noesis.ViewLocator" /> to configure.</param>
        protected virtual void ConfigureViewLocator(ViewLocator viewLocator)
        {
        }

        /// <summary>Override this to add custom logic on initialization.</summary>
        /// <returns><c>true</c> if the custom initialization logic was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool OnInitialize()
        {
            return true;
        }

        /// <summary>Override this to add custom logic on shutdown.</summary>
        protected virtual UniTask OnShutdown()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Override this to add custom logic on startup.</summary>
        protected virtual UniTask OnStartup()
        {
            return UniTask.CompletedTask;
        }

        #endregion

#if !UNITY_5_5_OR_NEWER
            var window = new Window
                             {
                                 Content = new ShellView(),
                                 Width = 1280,
                                 Height = 720
                             };
            window.Show();
            window.Closing += OnWindowClosing;
            await OnEnable();
#endif

            await OnStartup();

            if (!this.isInitialized)
            {
                return;
            }

            var canClose = true;

            if (WindowManager is IGuardClose guardClose)
            {
                canClose = await guardClose.CanCloseAsync();
            }

            args.Cancel = true;

            if (canClose)
            {
                await ShutdownAsync();
            }
        }

        #endregion

#endif

#if UNITY_5_5_OR_NEWER

        #region Unity Methods

        private void Awake()
        {
            this.noesisView = GetComponent<NoesisView>();

            if (!this.noesisView)
            {
                Logger.LogError("{Bootstrapper} must be on same game object as {NoesisView}", this, nameof(NoesisView));
            }
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid OnDestroy()
        {
            if (this.isInitialized)
            {
                Logger.LogWarning(
                    "Async shutdown logic not guaranteed to complete before {@GameObject} destruction - call and await {ShutdownAsync}() before destroying",
                    gameObject,
                    nameof(ShutdownAsync));

                await ShutdownAsync();
            }
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid OnDisable()
        {
            this.onDisableCancellation = new CancellationTokenSource();
            await OnDisableAsync(this.onDisableCancellation.Token);
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid OnEnable()
        {
            this.onEnableCancellation = new CancellationTokenSource();
            await OnEnableAsync(this.onEnableCancellation.Token);
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid Start()
        {
            await StartAsync();
        }

        #endregion

#endif

        #region Private Methods

        private void AddServiceProviderResource(ResourceDictionary resourceDictionary)
        {
            resourceDictionary[nameof(IServiceProvider)] = this;
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            ConfigureIoCContainer();

            using var _ = Logger.GetMethodTracer();

            Logger.LogInformation("Initializing...");

            ViewLocator = GetService<ViewLocator>();

            if (ViewLocator is null)
            {
                Logger.LogError(
                    "View locator not found - please register {ViewLocator} instance in DI container",
                    nameof(ViewLocator));
                Logger.LogError("Initialization failed");

                return;
            }

            Logger.LogInformation("Configuring type mappings");

            ViewLocator.ConfigureTypeMappings(new TypeMappingConfiguration());
            ConfigureViewLocator(ViewLocator);

            Logger.LogInformation("Resolving window manager");
            WindowManager = GetService<IWindowManager>();

            if (WindowManager is null)
            {
                Logger.LogError(
                    "Window manager not found - please register {IWindowManager} implementation in DI container",
                    nameof(IWindowManager));
                Logger.LogError("Initialization failed");

                return;
            }

            var wasOnInitializeSuccessful = OnInitialize();

            if (!wasOnInitializeSuccessful)
            {
                Logger.LogError("Initialization failed");

                return;
            }

            this.isInitialized = true;
            Logger.LogInformation("Initialization complete");
        }

        private async UniTask OnDisableAsync(CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(cancellationToken);

            await (this.onDisableCompletion?.Task ?? UniTask.CompletedTask);
            using var guard = TaskCompletion.CreateGuard(out this.onDisableCompletion);

            this.onEnableCancellation?.Cancel();
            await (this.onEnableCompletion?.Task ?? UniTask.CompletedTask);
            await (this.onStartCompletion?.Task ?? UniTask.CompletedTask);

            if (WindowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false, cancellationToken);
            }
        }

        private async UniTask OnEnableAsync(CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(cancellationToken);

            await (this.onEnableCompletion?.Task ?? UniTask.CompletedTask);
            using var guard = TaskCompletion.CreateGuard(out this.onEnableCompletion);

            this.onDisableCancellation?.Cancel();
            await (this.onDisableCompletion?.Task ?? UniTask.CompletedTask);

            if (WindowManager is IActivate activate)
            {
                await activate.ActivateAsync(cancellationToken);
            }
        }

        private async UniTask StartAsync()
        {
            using var _ = Logger.GetMethodTracer();

            if (this.onStartCompletion != null)
            {
                return;
            }

            using var guard = TaskCompletion.CreateGuard(out this.onStartCompletion);

            Initialize();

            if (!this.isInitialized)
            {
                return;
            }

            Logger.LogInformation("Starting...");

            await (this.onEnableCompletion?.Task ?? UniTask.CompletedTask);

#if UNITY_5_5_OR_NEWER
            var dictionary = this.noesisView.Content.Resources;
#else
            var shellView = new ShellView();
            var dictionary = shellView.Resources;
#endif

            Logger.LogInformation("Registering data templates");

            DataTemplateManager.RegisterDataTemplates(
                ViewLocator,
                dictionary,
                template => AddServiceProviderResource(template.Resources));

            AddServiceProviderResource(dictionary);

#if UNITY_5_5_OR_NEWER
            this.noesisView.Content.DataContext = WindowManager;
#else
            shellView.DataContext = WindowManager;
            var window = new Window { Width = 1280, Height = 720 };
            window.Show();
            window.Closing += OnWindowClosing;
            window.Content = shellView;
#endif

            await OnStartup();

            var mainContent = GetService<T>();

            Logger.LogInformation("Showing main content: {MainContent}", mainContent);

            await WindowManager.ShowMainContentAsync(mainContent);

            if (WindowManager is IActivate activate)
            {
                await activate.ActivateAsync();
            }

            Logger.LogInformation("Start complete");
        }

        #endregion
    }
}