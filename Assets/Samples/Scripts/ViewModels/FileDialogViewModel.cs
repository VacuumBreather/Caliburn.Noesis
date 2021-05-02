// <copyright file="FileDialogViewModel.cs" company="VacuumBreather">
//      Copyright © 2021 VacuumBreather. All rights reserved.
// </copyright>

namespace Caliburn.Noesis.Samples.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using Noesis;

    public class FileDialogViewModel : DialogScreen
    {
        #region Constants and Fields

        private FileInfo selectedFile;
        private DirectoryNode selectedDirectory;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public FileDialogViewModel()
        {
            DisplayName = "Open File";
            CloseCommand = new RelayCommand<bool?>(dialogResult => TryCloseAsync(dialogResult).Forget(), dialogResult => (dialogResult != true) || SelectedFile != null);
        }

        #endregion
        
        public FileInfo FileInfo { get; private set; }

        /// <inheritdoc />
        protected override UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            FileInfo = SelectedFile;
            Root.Clear();

            return UniTask.CompletedTask;
        }

        /// <inheritdoc />
        protected override async UniTask OnActivateAsync(CancellationToken cancellationToken)
        {
            var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            var rootNode = new RootNode(directoryInfo);
            Root.Add(rootNode);

            await rootNode.Initialize();
        }

        public RelayCommand<bool?> CloseCommand { get; }

        #region Public Properties

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

            public async UniTask Initialize()
            {
                await PopulateDirectories();
            }

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

            #endregion

            /// <inheritdoc />
            protected override void PotentiallyExpand(DirectoryInfo startingDirectory)
            {
                IsExpanded = true;
            }
        }

        #endregion
    }
}