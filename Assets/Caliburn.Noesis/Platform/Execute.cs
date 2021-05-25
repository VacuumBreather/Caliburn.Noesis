namespace Caliburn.Noesis
{
    using System;
    using Cysharp.Threading.Tasks;

#if !UNITY_5_5_OR_NEWER
    using System.Windows;
    using System.Windows.Threading;

#endif

    /// <summary>Enables easy marshalling of code to the UI thread.</summary>
    public static class Execute
    {
        #region Public Properties

#if !UNITY_5_5_OR_NEWER
        private static Dispatcher Dispatcher => Application.Current.Dispatcher;
#endif

        #endregion

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void BeginOnUIThread(this Action action)
        {
#if UNITY_5_5_OR_NEWER
            OnUIThreadAsync(
                () =>
                    {
                        action();

                        return UniTask.CompletedTask;
                    }).Forget();
#else
            Dispatcher.BeginInvoke(action);
#endif
        }

        #region Public Methods

        /// <summary>Executes the action on the UI thread.</summary>
        /// <param name="action">The action to execute.</param>
        public static void OnUIThread(this Action action)
        {
#if UNITY_5_5_OR_NEWER
            OnUIThreadAsync(
                    () =>
                        {
                            action();

                            return UniTask.CompletedTask;
                        })
                .AsTask()
                .Wait();
#else
            if (Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.Invoke(action);
            }
#endif
        }

        /// <summary>Executes the action on the UI thread asynchronously.</summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async UniTask OnUIThreadAsync(this Func<UniTask> action)
        {
#if UNITY_5_5_OR_NEWER
            await UniTask.SwitchToMainThread();
            await action();
#else
            await Dispatcher.InvokeAsync(action).Task.Unwrap();
#endif
        }

        #endregion
    }
}