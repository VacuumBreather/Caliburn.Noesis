namespace Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Caliburn.Noesis.Samples.ViewModels;
    using Cysharp.Threading.Tasks;

    #endregion

    public class DirectoryNode : FileSystemNode
    {
        #region Constructors and Destructors

        public DirectoryNode(DirectoryInfo directoryInfo, DirectoryInfo startingDirectory = null)
            : base(startingDirectory, directoryInfo.Name)
        {
            DirectoryInfo = directoryInfo;
        }

        #endregion

        #region Public Properties

        public DirectoryInfo DirectoryInfo { get; }

        #endregion

        #region Protected Methods

        /// <param name="startingDirectory"></param>
        /// <inheritdoc />
        protected override UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(DirectoryInfo startingDirectory)
        {
            IEnumerable<FileSystemNode> children;

            try
            {
                children = DirectoryInfo.EnumerateDirectories()
                                        .Where(
                                            directory =>
                                                (directory.Attributes &
                                                 (FileAttributes.Temporary |
                                                  FileAttributes.System |
                                                  FileAttributes.Hidden)) ==
                                                0)
                                        .Select(directory => new DirectoryNode(directory, startingDirectory))
                                        .ToList();
            }
            catch (SecurityException)
            {
                children = Array.Empty<FileSystemNode>();

                // Ignore
            }
            catch (UnauthorizedAccessException)
            {
                children = Array.Empty<FileSystemNode>();

                // Ignore
            }

            return UniTask.FromResult(children);
        }

        /// <inheritdoc />
        protected override UniTask<IEnumerable<FileInfo>> GetFilesAsync()
        {
            IEnumerable<FileInfo> files;

            try
            {
                files = DirectoryInfo.EnumerateFiles()
                                     .Where(
                                         file => (file.Attributes &
                                                  (FileAttributes.Temporary |
                                                   FileAttributes.System |
                                                   FileAttributes.Hidden)) ==
                                                 0)
                                     .ToList();
            }
            catch (SecurityException)
            {
                files = Array.Empty<FileInfo>();

                // Ignore
            }
            catch (UnauthorizedAccessException)
            {
                files = Array.Empty<FileInfo>();

                // Ignore
            }

            return UniTask.FromResult(files);
        }

        /// <inheritdoc />
        protected override void PotentiallyExpand(DirectoryInfo startingDirectory)
        {
            if (DirectoryInfo.IsSameAs(startingDirectory))
            {
                IsSelected = true;
            }

            if (DirectoryInfo.IsAncestorOf(startingDirectory))
            {
                IsExpanded = true;
            }
        }

        #endregion
    }
}