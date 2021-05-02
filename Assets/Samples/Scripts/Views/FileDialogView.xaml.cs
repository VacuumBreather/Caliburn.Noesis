namespace VacuumBreather.Montreal.MapEditor.Views
{
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using GUI = Noesis.GUI;
#if NOESIS
    using Noesis;

#else
    using System;
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Interaction logic for FileDialogView.xaml
    /// </summary>
    public partial class FileDialogView : UserControl
    {
        public FileDialogView()
        {
            InitializeComponent();
            Initialized += OnInitialized; 
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            var treeView = (TreeView)FindName("TreeView");
            treeView.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(OnTreeViewItemSelected));
        }

        #region Private Methods

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Scripts/MapEditor/Views/FileDialogView.xaml");
        }
#endif

        #endregion

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
#if NOESIS
            (e.Source as TreeViewItem)?.BringIntoView();
#else
            (e.OriginalSource as TreeViewItem)?.BringIntoView();
#endif
        }
    }
}
