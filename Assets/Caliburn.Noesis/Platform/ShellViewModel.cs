namespace Caliburn.Noesis
{
    #region Using Directives

    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    /// <summary>The root view model of the UI.</summary>
    [UsedImplicitly]
    public class ShellViewModel : Conductor<IConductor>.Collection.AllActive, IWindowManager
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel" /> class.</summary>
        public ShellViewModel()
            : base(true)
        {
            DisplayName = "RootConductor";
            MainContent.DisplayName = "MainConductor";
            DialogContent.DisplayName = "DialogConductor";
            WindowContent.DisplayName = "WindowConductor";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the conductor which hosts any dialog content.</summary>
        public DialogConductor DialogContent { get; } = new DialogConductor();

        /// <summary>Gets the conductor which hosts the main content.</summary>
        public Conductor<Screen> MainContent { get; } = new Conductor<Screen>();

        /// <summary>Gets the conductor which hosts windows.</summary>
        public Conductor<WindowScreen>.Collection.OneActive WindowContent { get; } =
            new Conductor<WindowScreen>.Collection.OneActive();

        #endregion

        #region IWindowManager Implementation

        /// <inheritdoc />
        public async UniTask<bool?> ShowDialogAsync(DialogScreen rootModel,
                                                    CancellationToken cancellationToken = default)
        {
            return await DialogContent.ShowDialogAsync(rootModel, cancellationToken);
        }

        /// <inheritdoc />
        public async UniTask ShowMainContentAsync(Screen rootModel,
                                                  CancellationToken cancellationToken = default)
        {
            await MainContent.ActivateItemAsync(rootModel, cancellationToken);
        }

        /// <inheritdoc />
        public UniTask ShowPopupAsync(PropertyChangedBase rootModel,
                                      CancellationToken cancellationToken = default)
        {
            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        public async UniTask ShowWindowAsync(WindowScreen rootModel,
                                       CancellationToken cancellationToken = default)
        {
            await WindowContent.ActivateItemAsync(rootModel, cancellationToken);
        }

        #endregion
    }
}