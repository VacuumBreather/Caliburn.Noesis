﻿using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// A base class for various implementations of <see cref="IConductor"/> that maintain an active item.
    /// </summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    public abstract class ConductorBaseWithActiveItem<T> : ConductorBase<T>, IConductActiveItem<T> where T : class
    {
        private T _activeItem;

        /// <summary>
        /// The currently active item.
        /// </summary>
        public T ActiveItem
        {
            get => _activeItem;
            set => ActivateItemAsync(value, CancellationToken.None);
        }

        /// <inheritdoc />
        object IHaveReadOnlyActiveItem.ActiveItem => ActiveItem;

        /// <summary>
        /// The currently active item.
        /// </summary>
        /// <value></value>
        object IHaveActiveItem.ActiveItem
        {
            get => ActiveItem;
            set => ActiveItem = (T)value;
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async UniTask ChangeActiveItemAsync(T newItem, bool closePrevious, CancellationToken cancellationToken)
        {
            var previousItem = _activeItem;
            newItem = EnsureItem(newItem);

            _activeItem = newItem;
            NotifyOfPropertyChange(nameof(ActiveItem));

            await ScreenExtensions.TryDeactivateAsync(previousItem, closePrevious, cancellationToken);

            if (IsActive)
                await ScreenExtensions.TryActivateAsync(newItem, cancellationToken);

            OnActivationProcessed(_activeItem, true);
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected UniTask ChangeActiveItemAsync(T newItem, bool closePrevious) => ChangeActiveItemAsync(newItem, closePrevious, default);
    }
}
