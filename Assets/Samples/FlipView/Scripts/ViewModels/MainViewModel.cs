namespace Caliburn.Noesis.Samples.FlipView.ViewModels
{
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>The main view-model of the sample.</summary>
    [UsedImplicitly]
    public class MainViewModel : Conductor<ChildViewModel>.Collection.OneActive
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        public MainViewModel()
        {
            Items.AddRange(Enumerable.Range(0, 15).Select(_ => new ChildViewModel()));
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            await ActivateItemAsync(Items.FirstOrDefault(), cancellationToken);
        }

        #endregion
    }
}