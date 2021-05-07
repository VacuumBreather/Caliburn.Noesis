namespace Caliburn.Noesis.Samples.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Controls;
#endif

    /// <summary>Interaction logic for SampleWindowView.xaml</summary>
    public partial class SampleWindowView : UserControl
    {
        #region Constructors and Destructors

        public SampleWindowView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

#if UNITY_5_5_OR_NEWER

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/Scripts/Views/SampleWindowView.xaml");
        }

        #endregion

#endif

        #endregion
    }
}