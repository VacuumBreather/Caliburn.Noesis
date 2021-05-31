namespace Caliburn.Noesis.Samples.MarkupExtensions.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    /// <summary>The main view-model of the sample.</summary>
    /// <seealso cref="Screen" />
    [PublicAPI]
    public class MainViewModel : Conductor<IMarkupExtensionSample>.Collection.OneActive
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        /// <param name="samples">The samples.</param>
        public MainViewModel(IEnumerable<IMarkupExtensionSample> samples)
        {
            Items.AddRange(samples);
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