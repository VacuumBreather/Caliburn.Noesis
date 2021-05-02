namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     A conductor for dialogs.
    /// </summary>
    public class DialogConductor : Conductor<DialogScreenBase>
    {
        #region Constants and Fields

        private UniTaskCompletionSource<bool?> taskCompletionSource;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Closes the specified dialog.
        /// </summary>
        /// <param name="dialog">The dialog to close.</param>
        /// <param name="dialogResult">The dialog result to pass up the chain.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        public async UniTask CloseDialogAsync(
            DialogScreenBase dialog,
            bool? dialogResult,
            CancellationToken cancellationToken = default)
        {
            await DeactivateItemAsync(dialog, true, cancellationToken);

            if (ActiveItem != dialog)
            {
                this.taskCompletionSource.TrySetResult(dialogResult);
            }
        }

        /// <summary>
        ///     Shows the specified <see cref="DialogScreenBase" /> as a dialog.
        /// </summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation.
        ///     The task result contains the dialog result.
        /// </returns>
        public async UniTask<bool?> ShowDialogAsync(
            DialogScreenBase dialog,
            CancellationToken cancellationToken = default)
        {
            this.taskCompletionSource = new UniTaskCompletionSource<bool?>();

            await ActivateItemAsync(dialog, cancellationToken);

            return await (ActiveItem == dialog ? this.taskCompletionSource.Task : UniTask.FromResult(default(bool?)));
        }

        #endregion
    }
}