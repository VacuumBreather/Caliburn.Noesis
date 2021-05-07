namespace Caliburn.Noesis
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows.Controls;

#endif

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

#if UNITY_5_5_OR_NEWER

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Caliburn.Noesis/Platform/ShellView.xaml");
        }

        #endregion

#endif

        #endregion
    }
}