namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.ComponentModel;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Denotes an instance which conducts other objects by managing an ActiveItem and maintaining a strict lifecycle.
    /// </summary>
    /// <remarks>
    ///     Conducted instances can opt in to the lifecycle by implementing any of the following <see cref="IActivate" />,
    ///     <see cref="IDeactivate" />, <see cref="IGuardClose" />.
    /// </remarks>
    public interface IConductor : IParent, INotifyPropertyChanged
    {
        /// <summary>
        ///     Occurs when an activation request is processed.
        /// </summary>
        event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        /// <summary>
        ///     Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        UniTask ActivateItemAsync(object item, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask DeactivateItemAsync(object item, bool close, CancellationToken cancellationToken = default);
    }

    /// <summary>
    ///     Denotes an instance which conducts other objects by managing an ActiveItem and maintaining a strict lifecycle.
    /// </summary>
    /// <remarks>
    ///     Conducted instances can opt in to the lifecycle by implementing any of the following <see cref="IActivate" />,
    ///     <see cref="IDeactivate" />, <see cref="IGuardClose" />.
    /// </remarks>
    /// <typeparam name="T">The type of item to conduct.</typeparam>
    public interface IConductor<in T> : IConductor
    {
        /// <summary>
        ///     Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of
        ///     cancellation.
        /// </param>
        UniTask ActivateItemAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask DeactivateItemAsync(T item, bool close, CancellationToken cancellationToken = default);
    }
}