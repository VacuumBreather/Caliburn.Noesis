namespace Caliburn.Noesis
{
    using System;
    using Cysharp.Threading.Tasks;
#if UNITY_5_5_OR_NEWER
    using GUI = global::Noesis.GUI;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using UnityEngine;

#else
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Threading;
#endif

    /// <summary>Inherit from this class to configure and run the framework.</summary>
#if UNITY_5_5_OR_NEWER
    [RequireComponent(typeof(NoesisView))]
#endif
    public abstract class BootstrapperBase<T>
#if UNITY_5_5_OR_NEWER
        : MonoBehaviour
#endif
        where T : Screen
    {
        #region Constants and Fields

        private bool isInitialized;
        private ILogger logger;
#if UNITY_5_5_OR_NEWER
        private NoesisView noesisView;
#endif
        private IWindowManager windowManager;
        private CaliburnConfiguration configuration;

        #endregion

#if UNITY_5_5_OR_NEWER

        #region Serialized Fields

        [SerializeField]
        [UsedImplicitly]
        private List<NoesisXaml> xamls;

        #endregion

#endif

#if !UNITY_5_5_OR_NEWER
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BootstrapperBase{T}" /> class.</summary>
        protected BootstrapperBase()
        {
            Execute.Dispatcher = Dispatcher.CurrentDispatcher;
            Application.Current.Startup += async (_, __) =>
                {
                    await Execute.OnUIThreadAsync(OnEnable);
                    await Execute.OnUIThreadAsync(Start);
                };
            Application.Current.Activated +=
                async (_, __) => await Execute.OnUIThreadAsync(OnEnable);
            Application.Current.Deactivated +=
                async (_, __) => await Execute.OnUIThreadAsync(OnDisable);
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

        #region Protected Methods

        /// <summary>Override this to return your own modified configuration.</summary>
        /// <returns>The <see cref="CaliburnConfiguration" /> to be used by the framework.</returns>
        protected virtual CaliburnConfiguration GetConfiguration()
        {
            return CaliburnConfiguration.Default;
        }

        /// <summary>
        ///     Override this to return the instance of your main content view-model. You can return an
        ///     instance retrieved from an IoC container here.
        /// </summary>
        /// <returns>The main content view-model.</returns>
        protected virtual T GetMainContentViewModel()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        ///     Override this to return your own implementation of <see cref="IWindowManager" />. You can
        ///     return an instance retrieved from an IoC container here.
        /// </summary>
        /// <returns>The <see cref="IWindowManager" /> instance.</returns>
        protected virtual IWindowManager GetWindowManager()
        {
            return this.windowManager ??= new ShellViewModel();
        }

        /// <summary>Override this to add custom behavior on exit.</summary>
        protected virtual UniTask OnExit()
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Override this to add custom behavior to execute after the application starts.</summary>
        protected virtual UniTask OnStartup()
        {
            return UniTask.CompletedTask;
        }

        #endregion

        #region Unity Methods

#if UNITY_5_5_OR_NEWER
        private void Awake()
        {
            this.noesisView = GetComponent<NoesisView>();

            if (!this.noesisView)
            {
                Logger.Error(
                    $"{GetType()} must be on the same game object as the {nameof(NoesisView)} component.");
            }
        }
#endif

#if UNITY_5_5_OR_NEWER
        private async void Start()
#else
        private async UniTask Start()
#endif
        {
#if !UNITY_5_5_OR_NEWER
            var window = new Window { Content = new ShellView(), Width = 1280, Height = 720 };
            window.Show();
            window.Closing += OnWindowClosing;
#endif

            await OnStartup();

            if (!this.isInitialized)
            {
                Logger.Error($"{GetType()} has not been initialized.");

                return;
            }

#if UNITY_5_5_OR_NEWER
            this.noesisView.Content.DataContext = this.windowManager;
#else
            window.DataContext = this.windowManager;
#endif

            var mainContent = GetMainContentViewModel();
            await this.windowManager.ShowMainContentAsync(mainContent);
        }

#if !UNITY_5_5_OR_NEWER
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

            args.Cancel = !canClose;

            if (canClose)
            {
                await Execute.OnUIThreadAsync(OnDestroy);
            }
        }
#endif

#if UNITY_5_5_OR_NEWER
        private async void OnEnable()
#else
        private async UniTask OnEnable()
#endif
        {
            Initialize();

            if (this.windowManager is IActivate activate)
            {
                await activate.ActivateAsync();
            }
        }

#if UNITY_5_5_OR_NEWER
        private async void OnDisable()
#else
        private async UniTask OnDisable()
#endif
        {
            if (this.windowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false);
            }
        }

#if UNITY_5_5_OR_NEWER
        private async void OnDestroy()
#else
        private async UniTask OnDestroy()
#endif
        {
            await OnExit();

            if (this.windowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(true);
            }
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.windowManager = GetWindowManager();
            this.configuration = GetConfiguration();

#if UNITY_5_5_OR_NEWER
            var dictionary = GUI.GetApplicationResources();
#else
            var dictionary = Application.Current.Resources;
#endif

            DataTemplateManager.RegisterDataTemplates(this.configuration, dictionary);

            this.isInitialized = true;
        }

        #endregion
    }
}