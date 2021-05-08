namespace Caliburn.Noesis.Samples.FileExplorer.Views
{
    using Extensions;
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
            this.InitializeComponent();
        }

        #endregion
    }
}