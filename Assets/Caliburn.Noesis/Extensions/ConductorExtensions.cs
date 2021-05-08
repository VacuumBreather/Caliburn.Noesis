namespace Caliburn.Noesis.Extensions
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>Provides extension methods for the <see cref="IConductor" /> type.</summary>
    public static class ConductorExtensions
    {
        #region Public Methods

        /// <summary>Activates the specified item.</summary>
        /// <param name="conductor">The conductor to activate the item with.</param>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask ActivateItemAsync(this IConductor conductor,
                                                object item,
                                                CancellationToken cancellationToken = default)
        {
            return conductor.ActivateItemAsync(item, cancellationToken);
        }

        /// <summary>Closes the specified item.</summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask CloseItemAsync(this IConductor conductor,
                                             object item,
                                             CancellationToken cancellationToken = default)
        {
            return conductor.DeactivateItemAsync(item, true, cancellationToken);
        }

        /// <summary>Closes the specified item.</summary>
        /// <param name="conductor">The conductor.</param>
        /// <param name="item">The item to close.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask CloseItemAsync<T>(this IConductor<T> conductor,
                                                T item,
                                                CancellationToken cancellationToken = default)
            where T : class
        {
            return conductor.DeactivateItemAsync(item, true, cancellationToken);
        }

        #endregion
    }
}