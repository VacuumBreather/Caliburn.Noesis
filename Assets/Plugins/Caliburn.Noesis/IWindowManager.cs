namespace Caliburn.Noesis
{
    #region Using Directives

    using Cysharp.Threading.Tasks;

    #endregion

    /// <summary>
    ///     A service that manages windows.
    /// </summary>
    public interface IWindowManager
    {
        /// <summary>
        ///     Shows a modal dialog for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        /// <returns>The dialog result.</returns>
        UniTask<bool?> ShowDialogAsync(DialogScreen rootModel);

        /// <summary>
        ///     Shows the specified model as the main content.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        UniTask ShowMainContentAsync(Screen rootModel);

        /// <summary>
        ///     Shows a popup at the current mouse position.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        UniTask ShowPopupAsync(PropertyChangedBase rootModel);

        /// <summary>
        ///     Shows a non-modal window for the specified model.
        /// </summary>
        /// <param name="rootModel">The root model.</param>
        UniTask ShowWindowAsync(Screen rootModel);
    }
}