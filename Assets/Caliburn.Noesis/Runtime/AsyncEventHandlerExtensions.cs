﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// AsyncEventHandlerExtensions class.
    /// </summary>
    /// <remarks>
    /// Contains helper functions to run Invoke methods asynchronously.
    /// </remarks>
    public static class AsyncEventHandlerExtensions
    {
        /// <summary>
        /// Gets the invocation list of the specified async event handler.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="handler">The async event handler.</param>
        /// <returns>An enumerable of async event handlers.</returns>
        public static IEnumerable<AsyncEventHandler<TEventArgs>> GetHandlers<TEventArgs>(
            this AsyncEventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
            => handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();

         /// <summary>
        /// Invokes all handlers of the specified async event handler asynchronously.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="handler">The async event handler.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <param name="cancellationToken">(Optional) The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the completion of all handler invocations.</returns>
        public static UniTask InvokeAllAsync<TEventArgs>(
            this AsyncEventHandler<TEventArgs> handler,
            object sender,
            TEventArgs e,
            CancellationToken cancellationToken = default)
            where TEventArgs : EventArgs
            => UniTask.WhenAll(
                handler.GetHandlers()
                    .Select(handleAsync => handleAsync(sender, e, cancellationToken)));
    }
}
