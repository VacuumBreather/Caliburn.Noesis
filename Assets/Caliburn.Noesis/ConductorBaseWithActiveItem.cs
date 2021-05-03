namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;

    #endregion

    /// <summary>
    ///     A base class for various implementations of <see cref="IConductor" /> that maintain an active item.
    /// </summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IConductActiveItem<T>
        where T : class
    {
        #region Constants and Fields

        private T activeItem;

        #endregion

        #region Public Properties

        /// <summary>
        ///     The currently active item.
        /// </summary>
        public T ActiveItem
        {
            get => this.activeItem;
            set => ActivateItemAsync(value, CancellationToken.None);
        }

        #endregion

        #region IHaveActiveItem Implementation

        /// <summary>
        ///     The currently active item.
        /// </summary>
        /// <value></value>
        object IHaveActiveItem.ActiveItem
        {
            get => ActiveItem;
            set => ActiveItem = (T)value;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async UniTask ChangeActiveItemAsync(
            T newItem,
            bool closePrevious,
            CancellationToken cancellationToken)
        {
            await ScreenExtensions.TryDeactivateAsync(this.activeItem, closePrevious, cancellationToken);

            newItem = EnsureItem(newItem);

            this.activeItem = newItem;

            NotifyOfPropertyChange(nameof(ActiveItem));

            if (IsActive)
            {
                await ScreenExtensions.TryActivateAsync(newItem, cancellationToken);
            }

            OnActivationProcessed(this.activeItem, true);
        }

        /// <summary>
        ///     Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected UniTask ChangeActiveItemAsync(T newItem, bool closePrevious)
        {
            return ChangeActiveItemAsync(newItem, closePrevious, CancellationToken.None);
        }

        #endregion
    }
}