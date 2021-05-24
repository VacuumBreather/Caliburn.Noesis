namespace Caliburn.Noesis.Samples.Transitions.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for SecondView.xaml.
    /// </summary>
    public partial class SecondView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecondView"/> class.
        /// </summary>
        public SecondView()
        {
            this.InitializeComponent();
        }
    }
}
