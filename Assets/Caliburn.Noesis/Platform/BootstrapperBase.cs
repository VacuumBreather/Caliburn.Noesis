﻿namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
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
        /// <remarks>Override this to specify a custom logger.</remarks>
        protected virtual ILogger Logger
        {
#if UNITY_5_5_OR_NEWER
            get => this.logger ??= LogManager.CreateLogger(this);
#else
            get => this.logger ??= LogManager.CreateLogger(GetType());
#endif
            set => this.logger = value;
        }

        #endregion

        #region IServiceProvider Implementation

        /// <inheritdoc />
        public virtual object GetService(Type serviceType)
        {
            return serviceType == typeof(IWindowManager)
                       ? new ShellViewModel()
                       : Activator.CreateInstance(serviceType);
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

            try
            {
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
        protected virtual void ConfigureIoCContainer(IEnumerable<Type> viewModelTypes)
        {
        }

        /// <summary>Override this to modify the configuration of the <see cref="ViewLocator" />.</summary>
        /// <param name="viewLocator">The <see cref="ViewLocator" /> to configure.</param>
        protected virtual void ConfigureViewLocator(ViewLocator viewLocator)
        {
        }

        /// <summary>Override this to add custom behavior on shutdown.</summary>
        protected virtual UniTask OnShutdown()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Override this to add custom behavior to execute after the application starts.</summary>
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
                Logger.Error(
                    $"{GetType()} must be on the same game object as the {nameof(NoesisView)} component.");
            }
        }

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid OnDestroy()
        {
            if (this.isInitialized)
            {
                Logger.Warn(
                    $"{this} was not shut down correctly. " +
                    $"{nameof(ShutdownAsync)}() will now be called by {nameof(OnDestroy)}().\n" +
                    "Any long running async shutdown logic is not guaranteed to be completed. " +
                    $"{nameof(ShutdownAsync)}() should be called and awaited before {this} is destroyed.");

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

            var viewLocator = new ViewLocator();
            ConfigureViewLocator(viewLocator);

            this.windowManager = GetService<IWindowManager>();

#if UNITY_5_5_OR_NEWER
            var dictionary = GUI.GetApplicationResources();
#else
            var dictionary = Application.Current.Resources;
#endif

            DataTemplateManager.RegisterDataTemplates(viewLocator, assemblySource, dictionary);

            this.isInitialized = true;
        }

        private async UniTask OnDisableAsync(CancellationToken cancellationToken)
        {
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

            if (this.windowManager is IActivate activate)
            {
                await activate.ActivateAsync(cancellationToken);
            }
        }

        private async UniTask StartAsync()
        {
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
                Logger.Error($"{this} has not been initialized.");

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