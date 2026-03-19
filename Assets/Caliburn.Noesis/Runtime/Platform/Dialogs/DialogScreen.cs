using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>A base class for dialog screens.</summary>
    public abstract class DialogScreen : Screen
    {
        private DialogResult _result = DialogResult.None;

        /// <summary>
        /// Gets the dialog result.
        /// </summary>
        internal DialogResult DialogResult => _result;

        /// <inheritdoc/>
        public sealed override UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.FromResult(true);
        }

        /// <summary>
        /// Closes the dialog with the given result.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public UniTask CloseDialogAsync(DialogResult dialogResult)
        {
            _result = dialogResult;

            return TryCloseAsync();
        }

        /// <inheritdoc/>
        public sealed override UniTask TryCloseAsync(bool? dialogResult = null)
        {
            return base.TryCloseAsync(dialogResult);
        }

        /// <inheritdoc/>
        protected sealed override async UniTask OnActivatedAsync(CancellationToken cancellationToken)
        {
            _result = DialogResult.None;

            await OnDialogOpenedAsync(cancellationToken);
            await base.OnActivatedAsync(cancellationToken);
        }

        /// <summary>
        /// Called when the dialog is opened.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual UniTask OnDialogOpenedAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <inheritdoc/>
        protected sealed override async UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsInitialized && close)
            {
                await OnDialogClosed(cancellationToken);
            }

            await base.OnDeactivateAsync(close, cancellationToken);
        }

        /// <summary>
        /// Called when the dialog is being closed.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual UniTask OnDialogClosed(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
