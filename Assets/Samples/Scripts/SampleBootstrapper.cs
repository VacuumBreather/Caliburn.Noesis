namespace Samples
{
    #region Using Directives

    using Caliburn.Noesis;
    using ViewModels;

    #endregion

    /// <summary>
    ///     The bootstrapper for the sample UI.
    /// </summary>
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