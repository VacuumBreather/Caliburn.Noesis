namespace Caliburn.Noesis
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    /// <summary>A base implementation of <see cref="IScreen" />.</summary>
    [PublicAPI]
    public abstract class Screen : ViewAware, IScreen, IChild
    {
        #region Constants and Fields

        private static ILogger logger;

        private UniTaskCompletionSource initializationCompletion;
        private UniTaskCompletionSource activateCompletion;
        private UniTaskCompletionSource deactivateCompletion;

        private CancellationTokenSource activateCancellation;
        private CancellationTokenSource deactivateCancellation;

        private string displayName;

        private bool isActive;
        private bool isInitialized;
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

        /// <summary>Gets or sets the <see cref="ILogger" /> for this instance.</summary>
        protected ILogger Logger
        {
            get => logger ??= LogManager.FrameworkLogger;
            private set => logger = value;
        }

        #endregion

        #region IActivate Implementation

        /// <inheritdoc />
        public event AsyncEventHandler<ActivationEventArgs> Activated;

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

            using var _ = Logger.GetMethodTracer(cancellationToken);

            using var guard = TaskCompletion.CreateGuard(out this.activateCompletion);
            this.activateCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this.deactivateCancellation?.Cancel();

            // Deactivation was cancelled but we will wait for all the synchronous steps to complete.
            await (this.deactivateCompletion?.Task ?? UniTask.CompletedTask);

            var initialized = false;

            if (!IsInitialized)
            {
                using var initGuard = TaskCompletion.CreateGuard(out this.initializationCompletion);

                Logger.LogDebug("Initializing {@Screen}...", this);

                // Deactivation is not allowed to cancel initialization, so we are only
                // passing the token that was passed to us.
                await OnInitializeAsync(cancellationToken);
                IsInitialized = initialized = true;
            }

            Logger.LogDebug("Activating {@Screen}...", this);
            await OnActivateAsync(this.activateCancellation.Token);

            IsActive = true;

            await RaiseActivatedAsync(initialized, this.activateCancellation.Token);

            if (this.activateCancellation?.IsCancellationRequested == true)
            {
                Logger.LogDebug("Activation of {@Screen} cancelled", this);
            }
        }

        #endregion

        #region IChild Implementation

        /// <inheritdoc />
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
        public event AsyncEventHandler<DeactivationEventArgs> Deactivating;

        /// <inheritdoc />
        async UniTask IDeactivate.DeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(close, cancellationToken);

            using var guard = TaskCompletion.CreateGuard(out this.deactivateCompletion);
            this.deactivateCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (!IsInitialized)
            {
                // We do not allow deactivation before initialization.
                await (this.initializationCompletion?.Task ?? UniTask.CompletedTask);
            }

            if (this.deactivateCancellation.IsCancellationRequested)
            {
                return;
            }

            this.activateCancellation?.Cancel();

            // Activation was cancelled but we will wait for all the synchronous steps to complete.
            await (this.activateCompletion?.Task ?? UniTask.CompletedTask);

            if (IsActive || (IsInitialized && close))
            {
                Logger.LogDebug("Deactivating {@Screen}...", this);
                await RaiseDeactivatingAsync(close, this.deactivateCancellation.Token);
                await OnDeactivateAsync(close, this.deactivateCancellation.Token);

                IsActive = false;

                await RaiseDeactivatedAsync(close, this.deactivateCancellation.Token);

                if (close)
                {
                    Logger.LogDebug("Closed {@Screen}", this);
                }
            }

            if (this.deactivateCancellation?.IsCancellationRequested == true)
            {
                Logger.LogDebug("Deactivation of {@Screen} cancelled", this);
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

        #region IHaveLogger Implementation

        /// <inheritdoc />
        ILogger IHaveLogger.Logger
        {
            get => Logger;
            set => Logger = value;
        }

        #endregion

        #region Protected Methods

        /// <summary>Called when activating.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        #region Private Methods

        private async UniTask RaiseActivatedAsync(bool wasInitialized,
                                                  CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(wasInitialized, cancellationToken);

            await (Activated?.InvokeAllAsync(
                       this,
                       new ActivationEventArgs { WasInitialized = wasInitialized },
                       cancellationToken) ??
                   UniTask.FromResult(true));
        }

        private async UniTask RaiseDeactivatedAsync(bool wasClosed,
                                                    CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(wasClosed, cancellationToken);

            await (Deactivated?.InvokeAllAsync(
                       this,
                       new DeactivationEventArgs { WasClosed = wasClosed },
                       cancellationToken) ??
                   UniTask.FromResult(true));
        }

        private async UniTask RaiseDeactivatingAsync(bool wasClosed,
                                                     CancellationToken cancellationToken)
        {
            using var _ = Logger.GetMethodTracer(wasClosed, cancellationToken);

            await (Deactivating?.InvokeAllAsync(
                       this,
                       new DeactivationEventArgs { WasClosed = wasClosed },
                       cancellationToken) ??
                   UniTask.FromResult(true));
        }

        #endregion
    }
}