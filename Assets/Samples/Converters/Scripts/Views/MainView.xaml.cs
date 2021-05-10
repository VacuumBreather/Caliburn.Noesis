namespace Caliburn.Noesis.Samples.Converters.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// The main view-model of the Converters sample.
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class MainView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
        }

#if UNITY_5_5_OR_NEWER

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/Converters/Scripts/Views/MainView.xaml");
        }

        #endregion

#endif
    }
}