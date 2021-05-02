namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Denotes an instance which may prevent closing.
    /// </summary>
    public interface IGuardClose : IClose
    {
        /// <summary>
        ///     Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation and contains the result of the close.</returns>
        UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default);
    }
}