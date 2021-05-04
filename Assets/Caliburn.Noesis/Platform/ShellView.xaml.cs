namespace Caliburn.Noesis
{
    #region Using Directives

    using global::Noesis;

    #endregion

    #region Using Directives

    #endregion

    /// <summary>Interaction logic for ShellView.xaml</summary>
    public partial class ShellView : Page
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ShellView" /> class.</summary>
        public ShellView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Caliburn.Noesis/Platform/ShellView.xaml");
        }

        #endregion
    }
}