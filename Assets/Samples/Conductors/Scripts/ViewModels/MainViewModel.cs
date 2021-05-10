namespace Caliburn.Noesis.Samples.Conductors.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>The main view-model of the Conductor.OneActive sample.</summary>
    public class MainViewModel : Conductor<ISubScreen>.Collection.OneActive
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        /// <param name="subScreens">The sub items of this screen.</param>
        public MainViewModel(IEnumerable<ISubScreen> subScreens)
        {
            Items.AddRange(subScreens.OrderBy(screen => screen.DisplayName));
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            await ActivateItemAsync(Items.FirstOrDefault(), cancellationToken);
        }

        #endregion
    }
}