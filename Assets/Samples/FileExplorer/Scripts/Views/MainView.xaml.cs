namespace Caliburn.Noesis.Samples.FileExplorer.Views
{
    using System;
    using global::Noesis;

#if UNITY_5_5_OR_NEWER

#else
    using System.Windows.Controls;
#endif

    /// <summary>Interaction logic for MainView.xaml</summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainView" /> class.</summary>
        public MainView()
        {
            InitializeComponent();
            var weakReference = new WeakReference(this);
            Loaded += (_, __) => ((MainView)weakReference.Target)?.OnLoaded();
        }

        #endregion

        #region Private Methods

        private static void OnTextChanged(TextBox textBox)
        {
            textBox.CaretIndex = Math.Min(textBox.Text.Length, textBox.Text.LastIndexOf('\n') + 1);
            textBox.ScrollToEnd();
        }

#if UNITY_5_5_OR_NEWER

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/FileExplorer/Scripts/Views/MainView.xaml");
        }

        #endregion

#endif

        private void OnLoaded()
        {
#if UNITY_5_5_OR_NEWER
            var textBox = (TextBox)FindName("TextContent");
#else
            var textBox = TextContent;
#endif

            textBox.TextChanged += (sender, __) => OnTextChanged((TextBox)sender);
        }

        #endregion
    }
}