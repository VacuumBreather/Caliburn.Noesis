namespace Caliburn.Noesis.Samples.FileExplorer.Views
{
#if UNITY_5_5_OR_NEWER
    using Extensions;
    using global::Noesis;

#else
    using System.Windows.Controls;
#endif

    /// <summary>Interaction logic for SampleWindowView.xaml</summary>
    public partial class SampleWindowView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="SampleWindowView" /> class.</summary>
        public SampleWindowView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}