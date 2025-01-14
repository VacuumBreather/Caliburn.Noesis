using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Denotes an instance which conducts other objects by managing an ActiveItem and maintaining a strict lifecycle.
    /// </summary>
    /// <remarks>Conducted instances can optin to the lifecycle by impelenting any of the follosing <see cref="IActivate"/>, <see cref="IDeactivate"/>, <see cref="IGuardClose"/>.</remarks>
    public interface IConductor : IParent, INotifyPropertyChangedEx
    {
        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
         /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        UniTask ActivateItemAsync(object item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask DeactivateItemAsync(object item, bool close, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Occurs when an activation request is processed.
        /// </summary>
        event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;
    }

    /// <summary>
    /// Denotes an instance which conducts other objects by managing an ActiveItem and maintaining a strict lifecycle.
    /// </summary>
    /// <remarks>Conducted instances can opt in to the lifecycle by implementing any of the following <see cref="IActivate" />, <see cref="IDeactivate" />, <see cref="IGuardClose" />.</remarks>
    /// <typeparam name="T">The type of item to conduct.</typeparam>
    public interface IConductor<T> : IConductor
    {
        /// <summary>
        /// Gets or sets the close strategy.
        /// </summary>
        /// <value>The close strategy.</value>
        ICloseStrategy<T> CloseStrategy { get; set; }

        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        UniTask ActivateItemAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to deactivate.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask DeactivateItemAsync(T item, bool close, CancellationToken cancellationToken = default);
    }

    /// <summary>An <see cref="IConductor" /> that also implements <see cref="IHaveActiveItem" />.</summary>
    public interface IConductActiveItem : IConductor, IHaveActiveItem
    {
    }

    /// <summary>An <see cref="IConductor{T}" /> that also implements <see cref="IHaveActiveItem" />.</summary>
    public interface IConductActiveItem<T> : IConductor<T>, IConductActiveItem
    {
    }
}
