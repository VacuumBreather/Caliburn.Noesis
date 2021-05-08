namespace Caliburn.Noesis.Samples.FileExplorer
{
    using ViewModels;

    /// <summary>The bootstrapper for the file explorer sample.</summary>
    public class FileExplorerBootstrapper : BootstrapperBase<MainViewModel>
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