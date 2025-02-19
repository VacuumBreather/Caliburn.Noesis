﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        bool HandlerExistsFor(Type messageType);

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        /// <param name = "marshal">Allows the subscriber to provide a custom thread marshaller for the message subscription.</param>
        void Subscribe(object subscriber, Func<Func<UniTask>, UniTask> marshal);

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        UniTask PublishAsync(object message, Func<Func<UniTask>, UniTask> marshal, CancellationToken cancellationToken = default);
    }
}
