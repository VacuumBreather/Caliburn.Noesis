namespace Caliburn.Noesis
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>Denotes an instance which may prevent closing.</summary>
    public interface IGuardClose : IClose
    {
        /// <summary>Called to check whether or not this instance can be closed.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains a value
        ///     indicating whether the instance can be closed.
        /// </returns>
        UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default);
    }
}