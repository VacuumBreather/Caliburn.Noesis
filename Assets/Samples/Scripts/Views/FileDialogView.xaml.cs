namespace Samples.Views
{
    #region Using Directives

    using Noesis;

    #endregion

    /// <summary>
    ///     Interaction logic for FileDialogView.xaml
    /// </summary>
    public partial class FileDialogView : UserControl
    {
        #region Constructors and Destructors

        public FileDialogView()
        {
            InitializeComponent();
            Initialized += OnInitialized;
        }

        #endregion

        #region Event Handlers

        private void OnInitialized(object sender, EventArgs args)
        {
            var treeView = (TreeView)FindName("TreeView");
            treeView.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(OnTreeViewItemSelected));
        }

        #endregion

        #region Private Methods

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Scripts/MapEditor/Views/FileDialogView.xaml");
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            (e.Source as TreeViewItem)?.BringIntoView();
        }

        #endregion
    }
}