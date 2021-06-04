namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using ILogger = Microsoft.Extensions.Logging.ILogger;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
    using UnityEngine;

#else
    using System.ComponentModel;
    using System.Windows;
#endif

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

        private CancellationTokenSource onEnableCancellation;
        private CancellationTokenSource onDisableCancellation;

#if UNITY_5_5_OR_NEWER
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
            Application.Current.Startup += async (_, __) =>
                {
                    Initialize();
                    this.onEnableCancellation = new CancellationTokenSource();
                    await OnEnableAsync(this.onEnableCancellation.Token);
                    await StartAsync();
                };
            Application.Current.Activated += async (_, __) =>
                {
                    this.onEnableCancellation = new CancellationTokenSource();
                    await OnEnableAsync(this.onEnableCancellation.Token);
                };
            Application.Current.Deactivated += async (_, __) =>
                {
                    this.onDisableCancellation = new CancellationTokenSource();
                    await OnDisableAsync(this.onDisableCancellation.Token);
                };
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

        private ViewLocator ViewLocator { get; } = new ViewLocator();

        private AssemblySource AssemblySource { get; } = new AssemblySource();

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
            catch (Exception)
            {
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

            if (!this.isInitialized)
            {
                return;
            }

            try
            {
                Logger.LogInformation("Shutting down...");

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
        /// <param name="viewModelTypes">
        ///     All relevant view-model types exported by the configured assemblies,
        ///     excluding any view-model implementing <see cref="IWindowManager" />.
        /// </param>
        /// <param name="viewTypes">
        ///     All relevant view types exported by the configured assemblies, excluding
        ///     <see cref="ShellView" />.
        /// </param>
        /// <remarks>
        ///     If you are configuring your own DI container you also need to override
        ///     <see cref="GetService" /> to let the framework use it. <br /> The following types need to be
        ///     registered here at a minimum:
        ///     <list type="bullet">
        ///         <item>
        ///             An <see cref="IWindowManager" /> implementation, using singleton lifetime in this
        ///             scope
        ///         </item>
        ///         <item>
        ///             Optionally a <see cref="ILoggerFactory" />, using singleton lifetime in this scope,
        ///             if you want to provide your own to the framework
        ///         </item>
        ///         <item>All relevant view, view-models and services</item>
        ///     </list>
        ///     The view-model implementing <see cref="IWindowManager" /> will not be part of the
        ///     <paramref name="viewModelTypes" /> sequence.
        /// </remarks>
        protected virtual void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes,
                                                     IEnumerable<Type> viewTypes)
        {
            IoCContainer = new Container();

            IoCContainer.RegisterSingleton<IWindowManager, ShellViewModel>();

#if UNITY_5_5_OR_NEWER
            IoCContainer.RegisterInstance(
                typeof(ILoggerFactory),
                new DebugLoggerFactory(this, LogLevel.Information));
#else
            IoCContainer.RegisterInstance(
                typeof(ILoggerFactory),
                new DebugLoggerFactory(LogLevel.Information));
#endif

            foreach (var viewType in viewTypes)
            {
                IoCContainer.RegisterPerRequest(viewType, viewType);
            }

            foreach (var type in viewModelTypes)
            {
                IoCContainer.RegisterPerRequest(type, type);

                foreach (var @interface in type.GetInterfaces())
                {
                    IoCContainer.RegisterPerRequest(@interface, type);
                }
            }
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

        /// <summary>
        ///     Override to tell the framework where to find assemblies to inspect for views, view-models,
        ///     etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
                       {
                           Assembly.GetExecutingAssembly(),
                           GetType().Assembly
                       };
        }

        #endregion

#if !UNITY_5_5_OR_NEWER
        #region Event Handlers

        private async void OnWindowClosing(object _, CancelEventArgs args)
        {
            if (args.Cancel)
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
                Logger.LogError(
                    "{Bootstrapper} must be on same game object as {NoesisView}",
                    this,
                    nameof(NoesisView));
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
            Initialize();
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

        private void AddImportantResources(ResourceDictionary resourceDictionary)
        {
            resourceDictionary[nameof(IServiceProvider)] = this;
            resourceDictionary[nameof(AssemblySource)] = AssemblySource;
            resourceDictionary[nameof(ViewLocator)] = ViewLocator;
        }

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            AssemblySource.AddRange(SelectAssemblies());

            ConfigureIoCContainer(AssemblySource.ViewModelTypes, AssemblySource.ViewTypes);

            if (GetService<ILoggerFactory>() is { } loggerFactory)
            {
                LogManager.SetLoggerFactory(loggerFactory);
            }

            using var _ = Logger.GetMethodTracer();

            Logger.LogInformation("Initializing...");

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
            using var guard = TaskCompletion.CreateGuard(out this.onStartCompletion);

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
                AssemblySource,
                dictionary,
                template => AddImportantResources(template.Resources));

            AddImportantResources(dictionary);

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

            Logger.LogInformation("Start complete");
        }

        #endregion
    }
}