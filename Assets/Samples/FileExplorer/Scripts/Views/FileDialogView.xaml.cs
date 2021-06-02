namespace Caliburn.Noesis.Samples.FileExplorer.Views
{
    using System;
#if UNITY_5_5_OR_NEWER
    using Extensions;
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>Interaction logic for FileDialogView.xaml</summary>
    public partial class FileDialogView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FileDialogView" /> class.</summary>
        public FileDialogView()
        {
            this.InitializeComponent();

            var weakReference = new WeakReference(this);
            Loaded += (_, __) => ((FileDialogView)weakReference.Target)?.OnLoaded();
        }

        #endregion

        #region Private Methods

        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
#if UNITY_5_5_OR_NEWER
            (e.Source as TreeViewItem)?.BringIntoView();
#else
            (e.OriginalSource as TreeViewItem)?.BringIntoView();
#endif
        }

        private void OnLoaded()
        {
#if UNITY_5_5_OR_NEWER
            var treeView = (TreeView)FindName("TreeView");
#else
            var treeView = TreeView;
#endif
            treeView.AddHandler(
                TreeViewItem.SelectedEvent,
                new RoutedEventHandler(OnTreeViewItemSelected));
        }

        #endregion
    }
}