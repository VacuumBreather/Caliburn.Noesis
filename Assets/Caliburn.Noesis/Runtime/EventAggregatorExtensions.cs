using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Extensions for <see cref="IEventAggregator"/>.
    /// </summary>
    public static class EventAggregatorExtensions
    {
        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />.
        /// </summary>
        /// <remarks>The subscription is invoked on the thread chosen by the publisher.</remarks>
        /// <param name="eventAggregator"></param>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public static void SubscribeOnPublishedThread(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.Subscribe(subscriber, f => f());
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />.
        /// </summary>
        /// <remarks>The subscription is invoked on the thread chosen by the publisher.</remarks>
        /// <param name="eventAggregator"></param>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        [Obsolete("Use SubscribeOnPublishedThread")]
        public static void Subscribe(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.SubscribeOnPublishedThread(subscriber);
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />.
        /// </summary>
        /// <remarks>The subscription is invoked on a new background thread.</remarks>
        /// <param name="eventAggregator"></param>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public static void SubscribeOnBackgroundThread(this IEventAggregator eventAggregator, object subscriber)
        {
#if UNITY_5_5_OR_NEWER
            eventAggregator.Subscribe(subscriber, f => UniTask.RunOnThreadPool(f));
#else
            eventAggregator.Subscribe(subscriber, f => UniTask.Run(f));
#endif
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />.
        /// </summary>
        /// <remarks>The subscription is invoked on the UI thread.</remarks>
        /// <param name="eventAggregator"></param>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public static void SubscribeOnUIThread(this IEventAggregator eventAggregator, object subscriber)
        {
            eventAggregator.Subscribe(subscriber, f =>
            {
                var taskCompletionSource = new UniTaskCompletionSource<bool>();

                Execute.BeginOnUIThread(async () =>
                {
                    try
                    {
                        await f();

                        taskCompletionSource.TrySetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                });

                return taskCompletionSource.Task;

            });
        }

        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnCurrentThreadAsync(this IEventAggregator eventAggregator, object message, CancellationToken cancellationToken)
        {
            return eventAggregator.PublishAsync(message, f => f(), cancellationToken);
        }

        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnCurrentThreadAsync(this IEventAggregator eventAggregator, object message)
        {
            return eventAggregator.PublishOnCurrentThreadAsync(message, default);
        }

        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnBackgroundThreadAsync(this IEventAggregator eventAggregator, object message, CancellationToken cancellationToken)
        {
#if UNITY_5_5_OR_NEWER
            return eventAggregator.PublishAsync(message, f => UniTask.RunOnThreadPool(f), cancellationToken);
#else
            return eventAggregator.PublishAsync(message, f => UniTask.Run(f), cancellationToken);
#endif
        }

        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnBackgroundThreadAsync(this IEventAggregator eventAggregator, object message)
        {
            return eventAggregator.PublishOnBackgroundThreadAsync(message, default);
        }

        /// <summary>
        /// Publishes a message on the UI thread.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnUIThreadAsync(this IEventAggregator eventAggregator, object message, CancellationToken cancellationToken)
        {
            return eventAggregator.PublishAsync(message, f =>
            {
                var taskCompletionSource = new UniTaskCompletionSource<bool>();

                Execute.BeginOnUIThread(async () =>
                {
                    try
                    {
                        await f();

                        taskCompletionSource.TrySetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        taskCompletionSource.TrySetCanceled();
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.TrySetException(ex);
                    }
                });

                return taskCompletionSource.Task;

            }, cancellationToken);
        }

        /// <summary>
        /// Publishes a message on the UI thread.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name = "message">The message instance.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static UniTask PublishOnUIThreadAsync(this IEventAggregator eventAggregator, object message)
        {
            return eventAggregator.PublishOnUIThreadAsync(message, default);
        }
    }
}
