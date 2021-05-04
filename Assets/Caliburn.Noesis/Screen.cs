namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;

    #endregion

    /// <summary>A base implementation of <see cref="IScreen" />.</summary>
    public abstract class Screen : PropertyChangedBase, IScreen, IChild
    {
        #region Constants and Fields

        private string displayName;
        private bool isActive;
        private bool isInitialized;
        private ILogger logger;
        private object parent;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Screen" /> class.</summary>
        protected Screen()
        {
            this.displayName = GetType().Name;
        }

        #endregion

        #region Public Properties

        /// <summary>Indicates whether or not this instance is currently initialized.</summary>
        public bool IsInitialized
        {
            get => this.isInitialized;
            private set => Set(ref this.isInitialized, value);
        }

        #endregion

        #region Protected Properties

        /// <summary>Gets the logger for this instance.</summary>
        protected ILogger Logger => this.logger ??= LogManager.GetLogger(this);

        #endregion

        #region IActivate Implementation

        /// <inheritdoc />
        public event EventHandler<ActivationEventArgs> Activated;

        /// <inheritdoc />
        public bool IsActive
        {
            get => this.isActive;
            private set => Set(ref this.isActive, value);
        }

        /// <inheritdoc />
        async UniTask IActivate.ActivateAsync(CancellationToken cancellationToken)
        {
            if (IsActive)
            {
                return;
            }

            var initialized = false;

            if (!IsInitialized)
            {
                await OnInitializeAsync(cancellationToken);
                IsInitialized = initialized = true;
            }

            Logger.Trace($"Activating {ToString()}.");

            await OnActivateAsync(cancellationToken);

            IsActive = true;

            Activated?.Invoke(this, new ActivationEventArgs { WasInitialized = initialized });
        }

        #endregion

        #region IChild Implementation

        /// <summary>Gets or sets the parent <see cref="IConductor" />.</summary>
        public object Parent
        {
            get => this.parent;
            set => Set(ref this.parent, value);
        }

        #endregion

        #region IClose Implementation

        /// <inheritdoc />
        public virtual async UniTask TryCloseAsync(bool? dialogResult = null)
        {
            if (Parent is IConductor conductor)
            {
                await conductor.CloseItemAsync(this, CancellationToken.None);
            }
        }

        #endregion

        #region IDeactivate Implementation

        /// <inheritdoc />
        public event AsyncEventHandler<DeactivationEventArgs> Deactivated;

        /// <inheritdoc />
        public event EventHandler<DeactivationEventArgs> Deactivating;

        /// <inheritdoc />
        async UniTask IDeactivate.DeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsActive || (IsInitialized && close))
            {
                Deactivating?.Invoke(this, new DeactivationEventArgs { WasClosed = close });

                Logger.Trace($"Deactivating {ToString()}.");
                await OnDeactivateAsync(close, cancellationToken);
#if UNIRX
                Subscriptions.Clear();
#endif
                IsActive = false;

                await (Deactivated?.InvokeAllAsync(
                           this,
                           new DeactivationEventArgs { WasClosed = close }) ??
                       UniTask.FromResult(true));

                if (close)
                {
#if UNIRX
                    Subscriptions.Dispose();
#endif
                    Logger.Trace($"Closed {ToString()}.");
                }
            }
        }

        #endregion

        #region IGuardClose Implementation

        /// <inheritdoc />
        public virtual UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.FromResult(true);
        }

        #endregion

        #region IHaveDisplayName Implementation

        /// <inheritdoc />
        public string DisplayName
        {
            get => this.displayName;
            set => Set(ref this.displayName, value);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return DisplayName;
        }

        #endregion

        #region Protected Methods

        /// <summary>Called when activating.</summary>
        protected virtual UniTask OnActivateAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Called when deactivating.</summary>
        /// <param name="close">Indicates whether this instance will be closed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>Called when initializing.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        #endregion
    }
}