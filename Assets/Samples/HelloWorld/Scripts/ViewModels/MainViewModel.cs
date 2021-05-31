namespace Caliburn.Noesis.Samples.HelloWorld.ViewModels
{
    using JetBrains.Annotations;

    /// <summary>The main view-model of the sample.</summary>
    /// <seealso cref="Screen" />
    [PublicAPI]
    public class MainViewModel : Screen
    {
        #region Public Properties

        /// <summary>Gets the name of the framework.</summary>
        public string FrameworkName => "Caliburn";

        #endregion
    }
}