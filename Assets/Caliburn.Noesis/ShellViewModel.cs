namespace Caliburn.Noesis
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>The root view model of the UI.</summary>
    [PublicAPI]
    public class ShellViewModel : Conductor<IConductor>.Collection.AllActive, IWindowManager
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ShellViewModel" /> class.</summary>
        public ShellViewModel()
            : base(true)
        {
            DisplayName = "RootConductor";
            MainContentConductor.DisplayName = "MainContentConductor";
            DialogConductor.DisplayName = "DialogConductor";
            WindowConductor.DisplayName = "WindowConductor";
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the conductor which hosts any dialog content.</summary>
        public DialogConductor DialogConductor { get; } = new DialogConductor();

        /// <summary>Gets the conductor which hosts the main content.</summary>
        public Conductor<Screen> MainContentConductor { get; } = new Conductor<Screen>();

        /// <summary>Gets the conductor which hosts windows.</summary>
        public WindowConductor WindowConductor { get; } = new WindowConductor();

        #endregion

        #region IWindowManager Implementation

        /// <inheritdoc />
        public async UniTask<bool?> ShowDialogAsync(DialogScreen rootModel,
                                                    CancellationToken cancellationToken = default)
        {
            using var _ = Logger.GetMethodTracer(rootModel, cancellationToken);

            return await DialogConductor.ShowDialogAsync(rootModel, cancellationToken);
        }

        /// <inheritdoc />
        public async UniTask ShowMainContentAsync(Screen rootModel, CancellationToken cancellationToken = default)
        {
            using var _ = Logger.GetMethodTracer(rootModel, cancellationToken);

            await MainContentConductor.ActivateItemAsync(rootModel, cancellationToken);
        }

        /// <inheritdoc />
        public UniTask ShowPopupAsync(BindableObject rootModel, CancellationToken cancellationToken = default)
        {
            using var _ = Logger.GetMethodTracer(rootModel, cancellationToken);

            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        public async UniTask ShowWindowAsync(WindowScreen rootModel, CancellationToken cancellationToken = default)
        {
            using var _ = Logger.GetMethodTracer(rootModel, cancellationToken);

            await WindowConductor.ActivateItemAsync(rootModel, cancellationToken);
        }

        #endregion
    }
}