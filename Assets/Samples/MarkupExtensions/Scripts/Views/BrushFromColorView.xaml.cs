namespace Caliburn.Noesis.Samples.MarkupExtensions.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
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