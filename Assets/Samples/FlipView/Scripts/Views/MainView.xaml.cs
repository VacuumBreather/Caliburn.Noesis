namespace Caliburn.Noesis.Samples.FlipView.Views
{
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    public partial class MainView : UserControl
    {
        public MainView()
        {
            this.InitializeComponent();
        }
    }
}