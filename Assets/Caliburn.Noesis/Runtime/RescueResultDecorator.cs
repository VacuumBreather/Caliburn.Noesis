﻿using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
using System.Windows;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    /// A result decorator which rescues errors from the decorated result by executing a rescue coroutine.
    /// </summary>
    /// <typeparam name="TException">The type of the exception we want to perform the rescue on</typeparam>
    public class RescueResultDecorator<TException> : ResultDecoratorBase where TException : Exception
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(RescueResultDecorator<>));
        private readonly bool cancelResult;
        private readonly Func<TException, IResult> coroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="RescueResultDecorator&lt;TException&gt;"/> class.
        /// </summary>
        /// <param name="result">The result to decorate.</param>
        /// <param name="coroutine">The rescue coroutine.</param>
        /// <param name="cancelResult">Set to true to cancel the result after executing rescue.</param>
        public RescueResultDecorator(IResult result, Func<TException, IResult> coroutine, bool cancelResult = true) : base(result)
        {
            this.coroutine = coroutine ?? throw new ArgumentNullException("coroutine");
            this.cancelResult = cancelResult;
        }

        /// <summary>
        /// Called when the execution of the decorated result has completed.
        /// </summary>
        /// <param name="methodContext">The context.</param>
        /// <param name="methodInnerResult">The decorated result.</param>
        /// <param name="args">The <see cref="ResultCompletionEventArgs" /> instance containing the event data.</param>
        protected override void OnInnerResultCompleted(CoroutineExecutionContext methodContext, IResult methodInnerResult, ResultCompletionEventArgs args)
        {
            var error = args.Error as TException;
            if (error == null)
            {
                OnCompleted(args);
            }
            else
            {
                Log.Error(error);
                Log.Info(string.Format("Executing coroutine because {0} threw an exception.", methodInnerResult.GetType().Name));
                Rescue(methodContext, error);
            }
        }

        private void Rescue(CoroutineExecutionContext context, TException exception)
        {
            IResult rescueResult;
            try
            {
                rescueResult = coroutine(exception);
            }
            catch (Exception ex)
            {
                OnCompleted(new ResultCompletionEventArgs { Error = ex });
                return;
            }

            try
            {
                rescueResult.Completed += RescueCompleted;
                context?.ServiceLocator?.BuildUp(rescueResult);
                rescueResult.Execute(context);
            }
            catch (Exception ex)
            {
                RescueCompleted(rescueResult, new ResultCompletionEventArgs { Error = ex });
            }
        }

        private void RescueCompleted(object sender, ResultCompletionEventArgs args)
        {
            ((IResult)sender).Completed -= RescueCompleted;
            OnCompleted(new ResultCompletionEventArgs
            {
                Error = args.Error,
                WasCancelled = (args.Error == null && (args.WasCancelled || cancelResult))
            });
        }
    }
}
