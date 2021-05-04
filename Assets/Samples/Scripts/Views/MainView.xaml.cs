namespace Samples.Views
{
    #region Using Directives

    using System;
    using Noesis;
    using EventArgs = Noesis.EventArgs;

    #endregion

    /// <summary>Interaction logic for MainView.xaml</summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainView" /> class.</summary>
        public MainView()
        {
            InitializeComponent();
            Initialized += OnInitialized;
        }

        #endregion

        #region Event Handlers

        private void OnInitialized(object sender, EventArgs args)
        {
            if (FindName("TextContent") is TextBox textBox)
            {
                textBox.TextChanged += (_, __) =>
                    {
                        textBox.CaretIndex = Math.Min(
                            textBox.Text.Length,
                            textBox.Text.LastIndexOf('\n') + 1);
                        textBox.ScrollToEnd();
                    };
            }
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/Scripts/Views/MainView.xaml");
        }

        #endregion
    }
}