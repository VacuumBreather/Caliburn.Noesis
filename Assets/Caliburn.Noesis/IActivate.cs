namespace Caliburn.Noesis
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>Denotes an instance which requires activation.</summary>
    public interface IActivate
    {
        /// <summary>Raised after activation occurs.</summary>
        event EventHandler<ActivationEventArgs> Activated;

        /// <summary>Indicates whether or not this instance is active.</summary>
        bool IsActive { get; }

        /// <summary>Activates this instance.</summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask ActivateAsync(CancellationToken cancellationToken = default);
    }
}