namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>A service that manages windows.</summary>
    public interface IWindowManager
    {
        /// <summary>Shows a modal dialog for the specified model.</summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        /// <returns>The dialog result.</returns>
        UniTask<bool?> ShowDialogAsync(DialogScreen rootModel,
                                       CancellationToken cancellationToken = default);

        /// <summary>Shows the specified model as the main content.</summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        UniTask ShowMainContentAsync(Screen rootModel,
                                     CancellationToken cancellationToken = default);

        /// <summary>Shows a popup at the current mouse position.</summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        UniTask ShowPopupAsync(PropertyChangedBase rootModel,
                               CancellationToken cancellationToken = default);

        /// <summary>Shows a non-modal window for the specified model.</summary>
        /// <param name="rootModel">The root model.</param>
        /// <param name="cancellationToken">
        ///     (Optional) A cancellation token that can be used by other objects
        ///     or threads to receive notice of cancellation.
        /// </param>
        UniTask ShowWindowAsync(WindowScreen rootModel, CancellationToken cancellationToken = default);
    }
}