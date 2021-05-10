namespace Caliburn.Noesis.Samples.FlipView.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;

#endif

    /// <summary>Interaction logic for ChildView.xaml</summary>
    public partial class ChildView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ChildView" /> class.</summary>
        public ChildView()
        {
            this.InitializeComponent();
        }

        #endregion
    }
}