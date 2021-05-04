namespace Caliburn.Noesis.Samples.Views
{
    #region Using Directives

#if UNITY_5_5_OR_NEWER
    using System;
    using global::Noesis;
    using EventArgs = global::Noesis.EventArgs;

#else
    using System;
    using System.Windows;
    using System.Windows.Controls;
#endif

    #endregion

    /// <summary>Interaction logic for FileDialogView.xaml</summary>
    public partial class FileDialogView : UserControl
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FileDialogView" /> class.</summary>
        public FileDialogView()
        {
            InitializeComponent();
#if UNITY_5_5_OR_NEWER
            var weakReference = new WeakReference(this);
            Initialized += (_, __) => ((FileDialogView)weakReference.Target)?.OnInitialized();
#else
            TreeView.AddHandler(
                TreeViewItem.SelectedEvent,
                new RoutedEventHandler(OnTreeViewItemSelected));
#endif
        }

        #endregion

        #region Private Methods

#if UNITY_5_5_OR_NEWER
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Samples/Scripts/Views/FileDialogView.xaml");
        }
#endif

#if UNITY_5_5_OR_NEWER

        #region Event Handlers

        private void OnInitialized()
        {
            var treeView = (TreeView)FindName("TreeView");
            treeView.AddHandler(
                TreeViewItem.SelectedEvent,
                new RoutedEventHandler(OnTreeViewItemSelected));
        }

        #endregion

#endif
        private static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
#if UNITY_5_5_OR_NEWER
            (e.Source as TreeViewItem)?.BringIntoView();
#else
            (e.OriginalSource as TreeViewItem)?.BringIntoView();
#endif
        }

        #endregion
    }
}