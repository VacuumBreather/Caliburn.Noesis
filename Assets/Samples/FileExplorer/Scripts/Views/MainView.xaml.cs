namespace Caliburn.Noesis.Samples.FileExplorer.Views
{
    using System;
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

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
            this.InitializeComponent();
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