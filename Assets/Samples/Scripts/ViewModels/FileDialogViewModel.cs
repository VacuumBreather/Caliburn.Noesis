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
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    /// <summary>
    ///     A view-model representing a dialog used to open files.
    /// </summary>
    public class FileDialogViewModel : DialogScreen
    {
        #region Constants and Fields

        private FileInfo selectedFile;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileDialogViewModel" /> class.
        /// </summary>
        public FileDialogViewModel()
        {
            DisplayName = "Open File";
            CloseDialogCommand = new RelayCommand<bool?>(
                dialogResult => TryCloseAsync(dialogResult).Forget(),
                dialogResult => (dialogResult != true) || (SelectedFile != null));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the opened file.
        /// </summary>
        public FileInfo FileInfo { get; private set; }

        /// <summary>
        ///     Gets the root node collection.
        /// </summary>
        [UsedImplicitly]
        public ObservableCollection<FileSystemNode> Root { get; } = new ObservableCollection<FileSystemNode>();

        /// <summary>
        ///     Gets the selected file.
        /// </summary>
        [UsedImplicitly]
        public FileInfo SelectedFile
        {
            get => this.selectedFile;
            set
            {
                Set(ref this.selectedFile, value);
                CloseDialogCommand.RaiseCanExecuteChanged();
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

        private class RootNode : FileSystemNode
        {
            /// <inheritdoc />
            public RootNode(DirectoryInfo startingDirectory)
                : base(startingDirectory, "Computer")
            {
            }

            /// <summary>
            ///     Initializes this instance.
            /// </summary>
            public async UniTask Initialize()
            {
                await PopulateDirectories();
            }

            /// <inheritdoc />
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
        }

        #endregion
    }
}