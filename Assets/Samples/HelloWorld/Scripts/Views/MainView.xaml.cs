namespace Caliburn.Noesis.Samples.HelloWorld.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Controls;
#endif

    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

#if UNITY_5_5_OR_NEWER

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/HelloWorld/Scripts/Views/MainView.xaml");
        }

        #endregion

#endif
    }
}