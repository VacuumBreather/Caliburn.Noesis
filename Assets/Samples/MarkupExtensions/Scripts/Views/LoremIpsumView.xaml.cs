namespace Caliburn.Noesis.Samples.MarkupExtensions.Views
{
#if UNITY_5_5_OR_NEWER
    using Extensions;
    using global::Noesis;

#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for LoremIpsumView.xaml</summary>
    public partial class LoremIpsumView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="LoremIpsumView" /> class.</summary>
        public LoremIpsumView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}