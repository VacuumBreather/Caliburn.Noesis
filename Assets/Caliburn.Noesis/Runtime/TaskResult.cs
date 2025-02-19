﻿using System;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>
    /// A coroutine that encapsulates an <see cref="UniTask"/>.
    /// </summary>
    public class TaskResult : IResult
    {
        private readonly UniTask innerTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskResult(UniTask task)
        {
            innerTask = task.Preserve();
        }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context)
        {
            if (innerTask.Status.IsCompleted())
            {
                OnCompleted(innerTask);
            }
            else
            {
                innerTask.ContinueWith(() => OnCompleted(innerTask));
            }
        }

        /// <summary>
        /// Called when the asynchronous task has completed.
        /// </summary>
        /// <param name="task">The completed task.</param>
        protected virtual async void OnCompleted(UniTask task)
        {
            Exception error = null;

            try
            {
                await task;
            }
            catch (Exception exception)
            {
                error = exception;
            }

            Completed(this, new ResultCompletionEventArgs { WasCancelled = task.Status.IsCanceled(), Error = error });
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
    /// <summary>
    /// A coroutine that encapsulates an <see cref="UniTask{TResult}"/>.
    /// </summary>
    public class TaskResult<TResult> : IResult<TResult>
    {
        private readonly UniTask<TResult> innerTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskResult{TResult}"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskResult(UniTask<TResult> task)
        {
            innerTask = task;
        }

        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        /// Executes the result using the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(CoroutineExecutionContext context)
        {
            if (innerTask.Status.IsCompleted())
            {
                OnCompleted(innerTask);
            }
            else
            {
                innerTask.ContinueWith(_ => OnCompleted(innerTask));
            }
        }

        /// <summary>
        /// Called when the asynchronous task has completed.
        /// </summary>
        /// <param name="task">The completed task.</param>
        protected virtual async void OnCompleted(UniTask<TResult> task)
        {
            Exception error = null;

            try
            {
                Result = await task;
            }
            catch (Exception exception)
            {
                error = exception;
            }

            Completed(this, new ResultCompletionEventArgs { WasCancelled = task.Status.IsCanceled(), Error = error });
        }

        /// <summary>
        /// Occurs when execution has completed.
        /// </summary>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };
    }
}
