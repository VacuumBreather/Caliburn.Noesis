﻿namespace Caliburn.Noesis
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>Denotes an instance which requires deactivation.</summary>
    public interface IDeactivate
    {
        /// <summary>Raised after deactivation.</summary>
        event AsyncEventHandler<DeactivationEventArgs> Deactivated;

        /// <summary>Raised before deactivation.</summary>
        event EventHandler<DeactivationEventArgs> Deactivating;

        /// <summary>Deactivates this instance.</summary>
        /// <param name="close">Indicates whether or not this instance is being closed.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask DeactivateAsync(bool close, CancellationToken cancellationToken = default);
    }
}