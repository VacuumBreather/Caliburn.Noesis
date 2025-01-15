using System.Threading;
using Cysharp.Threading.Tasks;

namespace Caliburn.Noesis
{
    /// <summary>Interface for a service for opening various dialogs and awaiting their results.</summary>
    public interface IDialogService : IHaveReadOnlyActiveItem<DialogScreen>, IScreen
    {
        /// <summary>Shows the specified <see cref="DialogScreen"/> as a dialog.</summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects or threads to receive
        ///     notice of cancellation.
        /// </param>
        /// <returns>A UniTask that represents the asynchronous save operation. The UniTask result contains the dialog result.</returns>
        UniTask<DialogResult> ShowDialogAsync(DialogScreen dialog, CancellationToken cancellationToken = default);
    }
}
