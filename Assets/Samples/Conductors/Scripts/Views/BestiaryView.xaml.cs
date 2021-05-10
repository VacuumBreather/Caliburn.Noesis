namespace Caliburn.Noesis.Samples.Conductors.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for BestiaryView.xaml</summary>
    public partial class BestiaryView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BestiaryView" /> class.</summary>
        public BestiaryView()
        {
            InitializeComponent();
        }

        #endregion
    }
}