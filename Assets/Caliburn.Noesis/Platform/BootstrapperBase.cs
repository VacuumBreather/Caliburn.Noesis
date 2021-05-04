namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;

    #endregion

    /// <summary>Inherit from this class to configure and run the framework.</summary>
    [RequireComponent(typeof(NoesisView))]
    public abstract class BootstrapperBase<T> : MonoBehaviour
        where T : Screen
    {
        #region Constants and Fields

        private bool isInitialized;

        private ILogger logger;
        private NoesisView noesisView;
        private IWindowManager windowManager;

        #endregion

        #region Serialized Fields

        [SerializeField]
        [UsedImplicitly]
        private List<NoesisXaml> xamls;

        #endregion

        #region Protected Properties

        /// <summary>Gets the <see cref="ILogger" /> for this instance.</summary>
        protected ILogger Logger => this.logger ??= LogManager.GetLogger(this);

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

        private void Awake()
        {
            this.windowManager = GetWindowManager();
            this.noesisView = GetComponent<NoesisView>();

            if (!this.noesisView)
            {
                Logger.Error(
                    $"{GetType()} must be on the same game object as the {nameof(NoesisView)} component.");
            }
        }

        private async void Start()
        {
            await OnStartup();

            Initialize(GetConfiguration());

            if (!this.isInitialized)
            {
                Logger.Error($"{GetType()} has not been initialized.");

                return;
            }

            this.noesisView.Content.DataContext = this.windowManager;
            var mainContent = GetMainContentViewModel();
            await this.windowManager.ShowMainContentAsync(mainContent);
        }

        private async void OnEnable()
        {
            if (this.windowManager is IActivate activate)
            {
                await activate.ActivateAsync();
            }
        }

        private async void OnDisable()
        {
            if (this.windowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(false);
            }
        }

        private async void OnDestroy()
        {
            await OnExit();

            if (this.windowManager is IDeactivate deactivate)
            {
                await deactivate.DeactivateAsync(true);
            }
        }

        #endregion

        #region Private Methods

        private void Initialize(CaliburnConfiguration configuration)
        {
            if (this.isInitialized)
            {
                return;
            }

            if (this.noesisView.Content is null)
            {
                Logger.Error($"The {nameof(NoesisView)} root XAML must be set.");

                return;
            }

            DataTemplateManager.RegisterDataTemplates(
                configuration,
                this.noesisView.Content.Resources);

            this.isInitialized = true;
        }

        #endregion
    }
}