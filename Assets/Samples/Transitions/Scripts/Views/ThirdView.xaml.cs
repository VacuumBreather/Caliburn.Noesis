namespace Caliburn.Noesis.Samples.Transitions.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for ThirdView.xaml
    /// </summary>
    public partial class ThirdView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdView"/> class.
        /// </summary>
        public ThirdView()
        {
            this.InitializeComponent();
        }
    }
}
