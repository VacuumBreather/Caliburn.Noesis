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

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogScreen"/> class.
        /// </summary>
        /// <param name="defaultResult">(Optional) The default result.</param>
        protected DialogScreen(DialogResult defaultResult = DialogResult.None)
        {
        }

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
        public async UniTask<DialogResult> GetDialogResultAsync()
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
        protected sealed override async UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (IsInitialized && close)
            {
                await _cancellationTokenRegistration!.Value.DisposeAsync();
                _cancellationTokenRegistration = null;
                _taskCompletionSource.TrySetResult(_result);
                _taskCompletionSource = null;
                IsInitialized = false;
            }

            await base.OnDeactivateAsync(close, cancellationToken);
        }

        /// <inheritdoc/>
        protected sealed override UniTask OnInitializedAsync(CancellationToken cancellationToken)
        {
            _taskCompletionSource = new UniTaskCompletionSource<DialogResult>();

            _cancellationTokenRegistration =
                cancellationToken.Register(() => _taskCompletionSource!.TrySetResult(result: default));

            return base.OnInitializedAsync(cancellationToken);
        }
    }
}
