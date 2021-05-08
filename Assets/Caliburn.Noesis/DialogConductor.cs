namespace Caliburn.Noesis
{
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>A conductor for dialogs.</summary>
    public class DialogConductor : Conductor<DialogScreen>
    {
        #region Constants and Fields

        private UniTaskCompletionSource<bool?> taskCompletionSource;

        #endregion

        #region Public Methods

        /// <summary>Closes the specified dialog.</summary>
        /// <param name="dialog">The dialog to close.</param>
        /// <param name="dialogResult">The dialog result to pass up the chain.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        public async UniTask CloseDialogAsync(DialogScreen dialog,
                                              bool? dialogResult,
                                              CancellationToken cancellationToken = default)
        {
            await DeactivateItemAsync(dialog, true, cancellationToken);

            if (ActiveItem != dialog)
            {
                this.taskCompletionSource.TrySetResult(dialogResult);
            }
        }

        /// <summary>Shows the specified <see cref="DialogScreen" /> as a dialog.</summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     dialog result.
        /// </returns>
        public async UniTask<bool?> ShowDialogAsync(DialogScreen dialog,
                                                    CancellationToken cancellationToken = default)
        {
            this.taskCompletionSource = new UniTaskCompletionSource<bool?>();

            await ActivateItemAsync(dialog, cancellationToken);

            return await (ActiveItem == dialog
                              ? this.taskCompletionSource.Task
                              : UniTask.FromResult(default(bool?)));
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnDeactivateAsync(bool close,
                                                           CancellationToken cancellationToken)
        {
            await ChangeActiveItemAsync(null, true, cancellationToken);

            this.taskCompletionSource?.TrySetResult(null);
        }

        #endregion
    }
}