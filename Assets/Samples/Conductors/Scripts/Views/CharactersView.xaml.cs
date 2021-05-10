namespace Caliburn.Noesis.Samples.Conductors.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for CharactersView.xaml
    /// </summary>
    public partial class CharactersView : UserControl
    {
        public CharactersView()
        {
            InitializeComponent();
        }
    }
}
