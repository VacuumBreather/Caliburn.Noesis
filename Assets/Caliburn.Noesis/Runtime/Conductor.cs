﻿using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// An implementation of <see cref="IConductor"/> that holds on to and activates only one item at a time.
    /// </summary>
    public partial class Conductor<T> : ConductorBaseWithActiveItem<T> where T : class
    {
        /// <inheritdoc />
        public override async UniTask ActivateItemAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item != null && item.Equals(ActiveItem))
            {
                if (IsActive)
                {
                    await ScreenExtensions.TryActivateAsync(item, cancellationToken);
                    OnActivationProcessed(item, true);
                }
                return;
            }

            var closeResult = await CloseStrategy.ExecuteAsync(new[] { ActiveItem }, cancellationToken);

            if (closeResult.CloseCanOccur)
            {
                await ChangeActiveItemAsync(item, true, cancellationToken);
            }
            else
            {
                OnActivationProcessed(item, false);
            }
        }

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async UniTask DeactivateItemAsync(T item, bool close, CancellationToken cancellationToken = default)
        {
            if (item == null || !item.Equals(ActiveItem))
            {
                return;
            }

            var closeResult = await CloseStrategy.ExecuteAsync(new[] { ActiveItem }, CancellationToken.None);

            if (closeResult.CloseCanOccur)
            {
                await ChangeActiveItemAsync(default(T), close, cancellationToken);
            }
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            var closeResult = await CloseStrategy.ExecuteAsync(new[] { ActiveItem }, cancellationToken);

            return closeResult.CloseCanOccur;
        }

        /// <summary>
        /// Called when view has been activated.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override UniTask OnActivatedAsync(CancellationToken cancellationToken)
        {
            return ScreenExtensions.TryActivateAsync(ActiveItem, cancellationToken);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Indicates whether this instance will be closed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return ScreenExtensions.TryDeactivateAsync(ActiveItem, close, cancellationToken);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The collection of children.</returns>
        public override IEnumerable<T> GetChildren()
        {
            return new[] { ActiveItem };
        }
    }
}
