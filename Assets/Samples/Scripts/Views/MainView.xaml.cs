namespace Samples.Views
{
    #region Using Directives

    using Noesis;

    #endregion

    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainView" /> class.
        /// </summary>
        public MainView()
        {
            InitializeComponent();
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