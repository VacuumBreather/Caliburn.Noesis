namespace Caliburn.Noesis.Samples.FlipView.ViewModels
{
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>Main view-model of the sample</summary>
    [UsedImplicitly]
    public class MainViewModel : Conductor<ChildViewModel>.Collection.OneActive
    {
        /// <inheritdoc />
        public MainViewModel()
        {
            Items.AddRange(Enumerable.Range(0,5).Select(_ => new ChildViewModel()));
        }

        /// <inheritdoc />
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            await ActivateItemAsync(Items.FirstOrDefault(), cancellationToken);
        }
    }
}