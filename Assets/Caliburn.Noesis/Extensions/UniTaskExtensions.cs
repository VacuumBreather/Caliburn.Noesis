namespace Caliburn.Noesis.Extensions
{
    using Cysharp.Threading.Tasks;

    /// <summary>Provides extension methods for the <see cref="UniTask" /> type.</summary>
    public static class UniTaskExtensions
    {
        #region Public Methods

        /// <summary>Uses the provided ticket to guard this task while it is running.</summary>
        /// <param name="task">The task to guard.</param>
        /// <param name="asyncGuard">The <see cref="AsyncGuard" /> to track the task with.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async UniTask Using(this UniTask task, AsyncGuard asyncGuard)
        {
            using (asyncGuard.GetToken())
            {
                await task;
            }
        }

        #endregion
    }
}