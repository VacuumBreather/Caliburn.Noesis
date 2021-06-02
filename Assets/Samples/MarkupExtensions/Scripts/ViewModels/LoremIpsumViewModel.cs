namespace Caliburn.Noesis.Samples.MarkupExtensions.ViewModels
{
    using JetBrains.Annotations;

    /// <summary>The view-model for the LoremIpsum sample.</summary>
    [PublicAPI]
    public class LoremIpsumViewModel : Screen, IMarkupExtensionSample
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LoremIpsumViewModel" /> class.</summary>
        public LoremIpsumViewModel()
        {
            DisplayName = "LoremIpsum";
        }

        #endregion
    }
}