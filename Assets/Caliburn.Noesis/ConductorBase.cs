namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>A base class for various implementations of <see cref="IConductor" />.</summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    public abstract class ConductorBase<T> : Screen, IConductor<T>, IParent<T>
        where T : class
    {
        #region Constants and Fields

        private ICloseStrategy<T> closeStrategy;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the close strategy.</summary>
        /// <value>The close strategy.</value>
        public ICloseStrategy<T> CloseStrategy
        {
            get => this.closeStrategy ?? (this.closeStrategy = new DefaultCloseStrategy<T>());
            set => this.closeStrategy = value;
        }

        #endregion

        #region IConductor Implementation

        /// <summary>Occurs when an activation request is processed.</summary>
        public virtual event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        UniTask IConductor.ActivateItemAsync(object item, CancellationToken cancellationToken)
        {
            return ActivateItemAsync((T)item, cancellationToken);
        }

        UniTask IConductor.DeactivateItemAsync(object item,
                                               bool close,
                                               CancellationToken cancellationToken)
        {
            return DeactivateItemAsync((T)item, close, cancellationToken);
        }

        #endregion

        #region IConductor<T> Implementation

        /// <summary>Activates the specified item.</summary>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads
        ///     to receive notice of cancellation.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public abstract UniTask ActivateItemAsync(T item,
                                                  CancellationToken cancellationToken = default);

        /// <summary>Deactivates the specified item.</summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public abstract UniTask DeactivateItemAsync(T item,
                                                    bool close,
                                                    CancellationToken cancellationToken = default);

        #endregion

        #region IParent Implementation

        IEnumerable IParent.GetChildren()
        {
            return GetChildren();
        }

        #endregion

        #region IParent<T> Implementation

        /// <summary>Gets the children.</summary>
        /// <returns>The collection of children.</returns>
        public abstract IEnumerable<T> GetChildren();

        #endregion

        #region Protected Methods

        /// <summary>Ensures that an item is ready to be activated.</summary>
        /// <param name="newItem">The item that is about to be activated.</param>
        /// <returns>The item to be activated.</returns>
        protected virtual T EnsureItem(T newItem)
        {
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