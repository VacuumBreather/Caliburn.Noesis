namespace Caliburn.Noesis.Samples.Conductors.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for QuestsView.xaml
    /// </summary>
    public partial class QuestsView : UserControl
    {
        public QuestsView()
        {
            InitializeComponent();
        }
    }
}
