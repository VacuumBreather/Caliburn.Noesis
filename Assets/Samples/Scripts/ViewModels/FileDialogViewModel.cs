namespace Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using Caliburn.Noesis;
    using Caliburn.Noesis.Samples.ViewModels;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    public class FileDialogViewModel : DialogScreen
    {
        #region Constants and Fields

        private DirectoryNode selectedDirectory;

        private FileInfo selectedFile;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public FileDialogViewModel()
        {
            DisplayName = "Open File";
            CloseCommand = new RelayCommand<bool?>(
                dialogResult => TryCloseAsync(dialogResult).Forget(),
                dialogResult => (dialogResult != true) || (SelectedFile != null));
        }

        #endregion

        #region Public Properties

        public RelayCommand<bool?> CloseCommand { get; }

        public FileInfo FileInfo { get; private set; }

        public ObservableCollection<FileSystemNode> Root { get; } = new ObservableCollection<FileSystemNode>();

        public FileInfo SelectedFile
        {
            get => this.selectedFile;
            [UsedImplicitly]
            set
            {
                Set(ref this.selectedFile, value);
                CloseCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override async UniTask OnActivateAsync(CancellationToken cancellationToken)
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            var rootNode = new RootNode(directoryInfo);
            Root.Add(rootNode);

            await rootNode.Initialize();
        }

        /// <inheritdoc />
        protected override UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            FileInfo = SelectedFile;
            Root.Clear();

            return UniTask.CompletedTask;
        }

        #endregion

        #region Nested Types

        public class RootNode : FileSystemNode
        {
            #region Constructors and Destructors

            /// <inheritdoc />
            public RootNode(DirectoryInfo startingDirectory)
                : base(startingDirectory, "Computer")
            {
            }

            #endregion

            #region Public Methods

            public async UniTask Initialize()
            {
                await PopulateDirectories();
            }

            #endregion

            #region Protected Methods

            protected override UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(DirectoryInfo startingDirectory)
            {
                IEnumerable<FileSystemNode> driveNodes;

                try
                {
                    driveNodes = DriveInfo.GetDrives()
                                          .Where(drive => drive.IsReady)
                                          .Select(drive => new DirectoryNode(drive.RootDirectory, StartingDirectory))
                                          .ToList();
                }
                catch (SecurityException)
                {
                    driveNodes = Array.Empty<FileSystemNode>();

                    // Ignore
                }
                catch (UnauthorizedAccessException)
                {
                    driveNodes = Array.Empty<FileSystemNode>();

                    // Ignore
                }

                return UniTask.FromResult(driveNodes);
            }

            /// <inheritdoc />
            protected override void PotentiallyExpand(DirectoryInfo startingDirectory)
            {
                IsExpanded = true;
            }

            #endregion
        }

        #endregion
    }
}