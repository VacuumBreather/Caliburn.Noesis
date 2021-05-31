namespace Caliburn.Noesis.Samples.MarkupExtensions.ViewModels
{
    using JetBrains.Annotations;

    /// <summary>
    /// The view-model for the BrushFromColor sample.
    /// </summary>
    [PublicAPI]
    public class BrushFromColorViewModel : Screen, IMarkupExtensionSample
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrushFromColorViewModel"/> class.
        /// </summary>
        public BrushFromColorViewModel()
        {
            DisplayName = "BrushFromColor";
        }
    }
}