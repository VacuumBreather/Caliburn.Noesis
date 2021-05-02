namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     Enables easy marshalling of code to the UI thread.
    /// </summary>
    public static class Execute
    {
        #region Public Methods

        /// <summary>
        ///     Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void OnUIThread(this Action action)
        {
            OnUIThreadAsync(
                    () =>
                        {
                            action();

                            return UniTask.CompletedTask;
                        })
                .Forget();
        }

        /// <summary>
        ///     Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async UniTask OnUIThreadAsync(this Func<UniTask> action)
        {
            await UniTask.SwitchToMainThread();
            await action();
        }

        #endregion
    }
}