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
#if UNITY_5_5_OR_NEWER
    using GUI = global::Noesis.GUI;
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
        private ILogger logger;

#if UNITY_5_5_OR_NEWER
        private NoesisView noesisView;
#endif

        private UniTaskCompletionSource onStartCompletion;
        private UniTaskCompletionSource onEnableCompletion;
        private UniTaskCompletionSource onDisableCompletion;

        private CancellationTokenSource onEnableCancellation;
        private CancellationTokenSource onDisableCancellation;

        private IWindowManager windowManager;

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

        /// <summary>Gets or sets the <see cref="ILogger" /> for this instance.</summary>
        protected ILogger Logger
        {
            get => this.logger ??= LogManager.FrameworkLogger;
            set => this.logger = value;
        }

        #endregion

        #region Private Properties

        private Container IoCContainer { get; set; }

        #endregion

        #region IServiceProvider Implementation

        /// <inheritdoc />
        public virtual object GetService(Type serviceType)
        {
            return IoCContainer.GetService(serviceType);
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
            if (!this.isInitialized)
            {
                return;
            }

            using var _ = Logger.GetMethodTracer();

            try
            {
                Logger.LogInformation("{Bootstrapper} shutting down", this);

                await (this.onStartCompletion?.Task ?? UniTask.CompletedTask);
                await (this.onDisableCompletion?.Task ?? UniTask.CompletedTask);

                this.onEnableCancellation?.Cancel();

                await OnShutdown();

                if (this.windowManager is IDeactivate deactivate)
                {
                    await deactivate.DeactivateAsync(true);
                }
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
        /// <param name="viewModelTypes">All relevant view-model types exported by the configured assemblies.</param>
        /// <remarks>
        ///     If you are configuring your own DI container you also need to override
        ///     <see cref="GetService" /> to let the framework use it. The following types need to be
        ///     registered here for the framework itself:
        ///     <list type="bullet">
        ///         <item>An <see cref="IWindowManager" /> implementation</item>
        ///         <item>The <see cref="ViewLocator" /></item> <item>The <see cref="NameTransformer" /></item>
        ///         <item>All relevant view models</item>
        ///     </list>
        /// </remarks>
        protected virtual void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes)
        {
            IoCContainer = new Container();

            IoCContainer.Register<IWindowManager>(typeof(ShellViewModel)).AsSingleton();
            IoCContainer.Register<ViewLocator>().AsSingleton();
            IoCContainer.Register<NameTransformer>().AsSingleton();

            foreach (var type in viewModelTypes)
            {
                IoCContainer.Register(type, type);

                foreach (var @interface in type.GetInterfaces())
                {
                    IoCContainer.Register(@interface, type);
                }
            }
        }

        /// <summary>Override this to modify the configuration of the <see cref="ViewLocator" />.</summary>
        /// <param name="viewLocator">The <see cref="ViewLocator" /> to configure.</param>
        protected virtual void ConfigureViewLocator(ViewLocator viewLocator)
        {
        }

        /// <summary>Override this to add custom logic on initialization.</summary>
        protected virtual void OnInitialize()
        {
#if UNITY_5_5_OR_NEWER
            LogManager.SetLoggerFactory(new DebugLoggerFactory(this, LogLevel.Information));
#else
            LogManager.SetLoggerFactory(new DebugLoggerFactory(LogLevel.Information));
#endif
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

        /// <summary>Override to tell the framework where to find assemblies to inspect for view-models, etc.</summary>
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

            if (this.windowManager is IGuardClose guardClose)
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
                    "{Bootstrapper} not shut down correctly - call {ShutdownAsync}() before destroying",
                    this,
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

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            var assemblySource = new AssemblySource();
            assemblySource.AddRange(SelectAssemblies());

            ConfigureIoCContainer(assemblySource.ViewModelTypes);

            var viewLocator = GetService<ViewLocator>();

            if (viewLocator is null)
            {
                Logger.LogError(
                    "{ViewLocator1} not found - please register {ViewLocator2} and {NameTransformer} in DI container",
                    nameof(ViewLocator),
                    nameof(ViewLocator),
                    nameof(NameTransformer));

                return;
            }

            viewLocator.ConfigureTypeMappings(new TypeMappingConfiguration());
            ConfigureViewLocator(viewLocator);

            this.windowManager = GetService<IWindowManager>();

            if (this.windowManager is null)
            {
                Logger.LogError(
                    "{IWindowManager1} not found - please register {IWindowManager2} implementation in DI container",
                    nameof(IWindowManager),
                    nameof(IWindowManager));

                return;
            }

#if UNITY_5_5_OR_NEWER
            var dictionary = GUI.GetApplicationResources();
#else
            var dictionary = Application.Current.Resources;
#endif

            DataTemplateManager.RegisterDataTemplates(viewLocator, assemblySource, dictionary);

            OnInitialize();

            this.isInitialized = true;

            Logger.LogInformation("{Bootstrapper} initialized", this);
        }

        private async UniTask OnDisableAsync(CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(cancellationToken);

            using var guard = new CompletionSourceGuard(out this.onDisableCompletion);
            this.onEnableCancellation?.Cancel();
            await (this.onEnableCompletion?.Task ?? UniTask.CompletedTask);
            await (this.onStartCompletion?.Task ?? UniTask.CompletedTask);

            if (this.windowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false, cancellationToken);
            }
        }

        private async UniTask OnEnableAsync(CancellationToken cancellationToken)
        {
            using var guard = new CompletionSourceGuard(out this.onEnableCompletion);

            this.onDisableCancellation?.Cancel();
            await (this.onDisableCompletion?.Task ?? UniTask.CompletedTask);

            Initialize();

            using var _ = Logger.GetMethodTracer(cancellationToken);

            if (this.windowManager is IActivate activate)
            {
                await activate.ActivateAsync(cancellationToken);
            }
        }

        private async UniTask StartAsync()
        {
            using var _ = Logger.GetMethodTracer();

            using var guard = new CompletionSourceGuard(out this.onStartCompletion);

            await (this.onEnableCompletion?.Task ?? UniTask.CompletedTask);

#if !UNITY_5_5_OR_NEWER
            var window = new Window { Content = new ShellView(), Width = 1280, Height = 720 };
            window.Show();
            window.Closing += OnWindowClosing;
#endif

            await OnStartup();

            if (!this.isInitialized)
            {
                Logger.LogError("{Bootstrapper} not initialized", this);

                return;
            }

#if UNITY_5_5_OR_NEWER
            this.noesisView.Content.DataContext = this.windowManager;
#else
            window.DataContext = this.windowManager;
#endif

            var mainContent = GetService<T>();
            await this.windowManager.ShowMainContentAsync(mainContent);
        }

        #endregion
    }
}