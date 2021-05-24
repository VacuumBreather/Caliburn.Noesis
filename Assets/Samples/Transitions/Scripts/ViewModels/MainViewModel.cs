namespace Caliburn.Noesis.Samples.Transitions.ViewModels
{
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>Main view-model of the transitions sample.</summary>
    [PublicAPI]
    public class MainViewModel : Conductor<Screen>.Collection.OneActive
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        public MainViewModel()
        {
            Items.Add(new FirstViewModel());
            Items.Add(new SecondViewModel());
            Items.Add(new ThirdViewModel());
            Items.Add(new ForthViewModel());
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            await ActivateItemAsync(Items.FirstOrDefault(), cancellationToken);

            await base.OnInitializeAsync(cancellationToken);
        }

        #endregion
    }
}