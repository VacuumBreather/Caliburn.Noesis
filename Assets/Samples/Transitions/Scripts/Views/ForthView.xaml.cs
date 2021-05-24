namespace Caliburn.Noesis.Samples.Transitions.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for ForthView.xaml.
    /// </summary>
    public partial class ForthView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForthView"/> class.
        /// </summary>
        public ForthView()
        {
            this.InitializeComponent();
        }
    }
}
