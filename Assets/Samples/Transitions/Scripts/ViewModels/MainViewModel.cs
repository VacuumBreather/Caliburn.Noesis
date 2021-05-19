namespace Caliburn.Noesis.Samples.Transitions.ViewModels
{
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>Main view-model of the sample</summary>
    [UsedImplicitly]
    public class MainViewModel : Conductor<Screen>.Collection.OneActive
    {
        #region Constants and Fields

        private int activeItemIndex;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        public MainViewModel()
        {
            Items.Add(new FirstViewModel());
            Items.Add(new SecondViewModel());

            GoToNextScreenCommand = new AsyncRelayCommand(GoToNextScreen);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the command to go to the next screen.</summary>
        /// <value>The command to go to the next screen.</value>
        [UsedImplicitly]
        public ICommand GoToNextScreenCommand { get; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            await ActivateItemAsync(Items.FirstOrDefault(), cancellationToken);

            this.activeItemIndex = Items.IndexOf(ActiveItem);

            await base.OnInitializeAsync(cancellationToken);
        }

        #endregion

        #region Private Methods

        private async UniTask GoToNextScreen()
        {
            this.activeItemIndex = ++this.activeItemIndex % Items.Count;

            await ActivateItemAsync(Items[this.activeItemIndex]);
        }

        #endregion
    }
}