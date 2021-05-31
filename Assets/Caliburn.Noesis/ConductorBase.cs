namespace Caliburn.Noesis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>A base class for various implementations of <see cref="IConductor" />.</summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    [PublicAPI]
    public abstract class ConductorBase<T> : Screen, IConductor<T>, IParent<T>
        where T : class
    {
        #region Constants and Fields

        private ICloseStrategy<T> closeStrategy;

        #endregion

        #region IConductor Implementation

        /// <inheritdoc />
        public virtual event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        /// <inheritdoc />
        UniTask IConductor.ActivateItemAsync(object item, CancellationToken cancellationToken)
        {
            return ActivateItemAsync((T)item, cancellationToken);
        }

        /// <inheritdoc />
        UniTask IConductor.DeactivateItemAsync(object item,
                                               bool close,
                                               CancellationToken cancellationToken)
        {
            return DeactivateItemAsync((T)item, close, cancellationToken);
        }

        #endregion

        #region IConductor<T> Implementation

        /// <summary>Gets or sets the close strategy.</summary>
        /// <value>The close strategy.</value>
        public ICloseStrategy<T> CloseStrategy
        {
            get => this.closeStrategy ?? (this.closeStrategy = new DefaultCloseStrategy<T>());
            set => this.closeStrategy = value;
        }

        /// <inheritdoc />
        public abstract UniTask ActivateItemAsync(T item,
                                                  CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public abstract UniTask DeactivateItemAsync(T item,
                                                    bool close,
                                                    CancellationToken cancellationToken = default);

        #endregion

        #region IParent Implementation

        /// <inheritdoc />
        IEnumerable IParent.GetChildren()
        {
            return GetChildren();
        }

        #endregion

        #region IParent<T> Implementation

        /// <inheritdoc />
        public abstract IEnumerable<T> GetChildren();

        #endregion

        #region Protected Methods

        /// <summary>Ensures that an item is ready to be activated.</summary>
        /// <param name="newItem">The item that is about to be activated.</param>
        /// <returns>The item to be activated.</returns>
        protected virtual T EnsureItem(T newItem)
        {
            using var _ = Logger.GetMethodTracer(newItem);

            if (newItem is IChild child && (child.Parent != this))
            {
                child.Parent = this;
            }

            return newItem;
        }

        /// <summary>Called by a subclass when an activation needs processing.</summary>
        /// <param name="item">The item on which activation was attempted.</param>
        /// <param name="success">If set to <c>true</c> the activation was successful.</param>
        protected virtual void OnActivationProcessed(T item, bool success)
        {
            using var _ = Logger.GetMethodTracer(item, success);

            if (item == null)
            {
                return;
            }

            ActivationProcessed?.Invoke(
                this,
                new ActivationProcessedEventArgs { Item = item, Success = success });
        }

        #endregion
    }
}