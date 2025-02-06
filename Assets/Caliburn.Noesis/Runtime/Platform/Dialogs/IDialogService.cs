using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>Interface for a service for opening various dialogs and awaiting their results.</summary>
    public interface IDialogService : IHaveReadOnlyActiveItem<DialogScreen>, IScreen
    {
        /// <summary>Shows the specified <see cref="DialogScreen" /> as a dialog.</summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.
        /// </param>
        /// <returns>A UniTask that represents the asynchronous save operation. The UniTask result contains the dialog result.</returns>
        UniTask<DialogResult> ShowDialogAsync(DialogScreen dialog, CancellationToken cancellationToken = default);

        /// <summary>Shows the specified <see cref="DialogScreen" /> as a dialog.</summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content text of the dialog.</param>
        /// <param name="dialogResults">The possible results the dialog can return.</param>
        /// <param name="defaultResult">(Optional) The default result.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.
        /// </param>
        /// <returns>A UniTask that represents the asynchronous save operation. The UniTask result contains the dialog result.</returns>
        UniTask<DialogResult> ShowQueryDialogAsync(string title,
            string content,
            DialogResults dialogResults,
            DialogResult defaultResult = DialogResult.None, CancellationToken cancellationToken = default);

        /// <summary>Shows the specified <see cref="DialogScreen" /> as a dialog.</summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="content">The content text of the dialog.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.
        /// </param>
        /// <returns>A UniTask that represents the asynchronous save operation. The UniTask result contains the dialog result.</returns>
        UniTask<DialogResult> ShowInformationDialogAsync(string title, string content,
            CancellationToken cancellationToken = default);
    }
}
