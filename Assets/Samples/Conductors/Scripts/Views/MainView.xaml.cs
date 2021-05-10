namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for MainView.xaml</summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainView" /> class.</summary>
        public MainView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}