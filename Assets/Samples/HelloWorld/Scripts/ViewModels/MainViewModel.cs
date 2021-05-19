namespace Caliburn.Noesis.Samples.HelloWorld.ViewModels
{
    using JetBrains.Annotations;

    /// <summary>Main view-model of the sample</summary>
    [UsedImplicitly]
    public class MainViewModel : Screen
    {
        #region Public Properties

        /// <summary>Gets the name of the framework.</summary>
        public string FrameworkName => "Caliburn";

        #endregion
    }
}