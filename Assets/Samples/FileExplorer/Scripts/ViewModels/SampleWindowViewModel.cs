namespace Caliburn.Noesis.Samples.FileExplorer.ViewModels
{
    /// <summary>The view-model for a sample window.</summary>
    public class SampleWindowViewModel : WindowScreen
    {
        #region Constants and Fields

        private static int windowIndex;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public SampleWindowViewModel()
        {
            DisplayName = $"Window #{windowIndex++}";
        }

        #endregion
    }
}