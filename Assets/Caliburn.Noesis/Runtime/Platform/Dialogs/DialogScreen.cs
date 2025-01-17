using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>A base class for dialog screens.</summary>
    public abstract class DialogScreen : Screen
    {
        private UniTaskCompletionSource<DialogResult> _taskCompletionSource;
        private CancellationTokenRegistration? _cancellationTokenRegistration;
        private DialogResult _result = DialogResult.None;

        /// <inheritdoc/>
        public sealed override UniTask<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            return UniTask.FromResult(true);
        }

        /// <summary>Gets the result this dialog was closed with.</summary>
        /// <returns>
        ///     A <see cref="UniTask"/> representing the asynchronous operation. The UniTask result contains the result
        ///     this dialog was closed with.
        /// </returns>
        /// <exception cref="InvalidOperationException">Attempting to await the result before initializing the dialog.</exception>
        internal async UniTask<DialogResult> GetDialogResultAsync()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException(
                    $"It was attempted to await the dialog result of {GetType().Name} before initializing it.");
            }

            return await _taskCompletionSource!.Task;
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
#if UNITY_5_5_OR_NEWER
                await _cancellationTokenRegistration!.Value.DisposeAsync();
#else
                _cancellationTokenRegistration!.Value.Dispose();
#endif
                _cancellationTokenRegistration = null;
                _taskCompletionSource.TrySetResult(_result);
                _taskCompletionSource = null;
                IsInitialized = false;

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

        /// <inheritdoc/>
        protected sealed override async UniTask OnInitializedAsync(CancellationToken cancellationToken)
        {
            _taskCompletionSource = new UniTaskCompletionSource<DialogResult>();

            _cancellationTokenRegistration =
                cancellationToken.Register(() => _taskCompletionSource!.TrySetResult(result: default));

            await base.OnInitializedAsync(cancellationToken);
        }
    }
}
