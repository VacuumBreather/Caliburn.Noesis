namespace Caliburn.Noesis
{
    using System;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// Extension methods to bring <see cref="UniTask"/> and <see cref="IResult"/> together.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Executes an <see cref="Caliburn.Noesis.IResult"/> asynchronous.
        /// </summary>
        /// <param name="result">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <returns>A task that represents the asynchronous coroutine.</returns>
        public static UniTask ExecuteAsync(this IResult result, CoroutineExecutionContext context = null)
        {
            return InternalExecuteAsync<object>(result, context);
        }

        /// <summary>
        /// Executes an <see cref="Caliburn.Noesis.IResult&lt;TResult&gt;"/> asynchronous.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="result">The coroutine to execute.</param>
        /// <param name="context">The context to execute the coroutine within.</param>
        /// <returns>A task that represents the asynchronous coroutine.</returns>
        public static UniTask<TResult> ExecuteAsync<TResult>(this IResult<TResult> result,
                                                          CoroutineExecutionContext context = null)
        {
            return InternalExecuteAsync<TResult>(result, context);
        }

        private static UniTask<TResult> InternalExecuteAsync<TResult>(IResult result, CoroutineExecutionContext context)
        {
            var taskSource = new UniTaskCompletionSource<TResult>();

            EventHandler<ResultCompletionEventArgs> completed = null;
            completed = (s, e) =>
            {
                result.Completed -= completed;

                if (e.Error != null)
                {
                    taskSource.TrySetException(e.Error);
                }
                else if (e.WasCancelled)
                {
                    taskSource.TrySetCanceled();
                }
                else
                {
                    var rr = result as IResult<TResult>;
                    taskSource.TrySetResult(rr != null ? rr.Result : default(TResult));
                }
            };

            try
            {
                result.Completed += completed;
                result.Execute(context ?? new CoroutineExecutionContext());
            }
            catch (Exception ex)
            {
                result.Completed -= completed;
                taskSource.TrySetException(ex);
            }

            return taskSource.Task;
        }

        /// <summary>
        /// Encapsulates a task inside a couroutine.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>The coroutine that encapsulates the task.</returns>
        public static TaskResult AsResult(this UniTask task)
        {
            return new TaskResult(task);
        }

        /// <summary>
        /// Encapsulates a task inside a couroutine.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The coroutine that encapsulates the task.</returns>
        public static TaskResult<TResult> AsResult<TResult>(this UniTask<TResult> task)
        {
            return new TaskResult<TResult>(task);
        }
    }
}
