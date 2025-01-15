using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>A conductor handling dialogs.</summary>
    public sealed class DialogConductor : Conductor<DialogScreen>.Collection.OneActive, IDialogService
    {
        /// <summary>Initializes a new instance of the <see cref="DialogConductor"/> class.</summary>
        public DialogConductor()
        {
            DisplayName = GetType().Name;
        }

        /// <inheritdoc/>
        object IHaveReadOnlyActiveItem.ActiveItem => ActiveItem;

        /// <summary>Shows the specified <see cref="DialogScreen"/> as a dialog.</summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.
        /// </param>
        /// <returns>
        ///     A <see cref="UniTask"/> that represents the asynchronous save operation. The <see cref="UniTask"/> result
        ///     contains the dialog result.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Attempting to open a dialog with the same instance multiple times
        ///     simultaneously.
        /// </exception>
        public async UniTask<DialogResult> ShowDialogAsync(DialogScreen dialog,
                                                             CancellationToken cancellationToken = default)
        {
            if (!Items.Contains(dialog))
            {
                throw new ArgumentException(
                    $"Attempting to open a {dialog.GetType().Name} dialog with the same instance multiple times simultaneously.",
                    nameof(dialog));
            }

            await ActivateItemAsync(dialog, cancellationToken);

            return await dialog.GetDialogResultAsync();
        }
    }
}
