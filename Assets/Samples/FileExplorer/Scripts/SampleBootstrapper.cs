namespace Caliburn.Noesis.Samples.FileExplorer
{
    using ViewModels;

    /// <summary>The bootstrapper for the sample UI.</summary>
    public class SampleBootstrapper : BootstrapperBase<MainViewModel>
    {
        #region Protected Methods

        /// <inheritdoc />
        protected override MainViewModel GetMainContentViewModel()
        {
            return new MainViewModel(GetWindowManager());
        }

        #endregion
    }
}