namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    /// <summary>
    ///     The root view model of the UI.
    /// </summary>
    [UsedImplicitly]
    public class ShellViewModel : Conductor<IConductor>.Collection.AllActive, IWindowManager
    {
        #region Constructors and Destructors

        /// <inheritdoc />
        public ShellViewModel()
            : base(true)
        {
            DisplayName = "Root Conductor";
            MainContent.DisplayName = "Main Content Conductor";
            DialogContent.DisplayName = "Dialog Conductor";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the conductor which hosts any dialog content.
        /// </summary>
        public DialogConductor DialogContent { get; } = new DialogConductor();

        /// <summary>
        ///     Gets the conductor which hosts the main content.
        /// </summary>
        public Conductor<Screen> MainContent { get; } = new Conductor<Screen>();

        #endregion

        #region IWindowManager Implementation

        /// <inheritdoc />
        public async UniTask<bool?> ShowDialogAsync(DialogScreen rootModel)
        {
            return await DialogContent.ShowDialogAsync(rootModel, CancellationToken.None);
        }

        /// <inheritdoc />
        public async UniTask ShowMainContentAsync(Screen rootModel)
        {
            await MainContent.ActivateItemAsync(rootModel, CancellationToken.None);
        }

        /// <inheritdoc />
        public UniTask ShowPopupAsync(PropertyChangedBase rootModel)
        {
            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        public UniTask ShowWindowAsync(Screen rootModel)
        {
            return UniTask.CompletedTask;
        }

        #endregion
    }
}