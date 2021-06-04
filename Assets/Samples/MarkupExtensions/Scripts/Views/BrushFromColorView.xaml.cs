namespace Caliburn.Noesis.Samples.MarkupExtensions.Views
{
#if UNITY_5_5_OR_NEWER
    using Extensions;
    using global::Noesis;

#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for BrushFromColorView.xaml</summary>
    public partial class BrushFromColorView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="BrushFromColorView" /> class.</summary>
        public BrushFromColorView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}