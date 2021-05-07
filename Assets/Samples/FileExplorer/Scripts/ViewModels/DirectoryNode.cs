namespace Caliburn.Noesis.Samples.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;
    using Cysharp.Threading.Tasks;

    /// <summary>A <see cref="FileSystemNode" /> representing a directory.</summary>
    public class DirectoryNode : FileSystemNode
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DirectoryNode" /> class.</summary>
        /// <param name="directoryInfo">The <see cref="DirectoryInfo" /> to wrap.</param>
        /// <param name="startingDirectory">The starting directory of the tree.</param>
        public DirectoryNode(DirectoryInfo directoryInfo, DirectoryInfo startingDirectory = null)
            : base(startingDirectory, directoryInfo.Name)
        {
            DirectoryInfo = directoryInfo;
        }

        #endregion

        #region Public Properties

        /// <summary>The wrapped <see cref="DirectoryInfo" />.</summary>
        public DirectoryInfo DirectoryInfo { get; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(
            DirectoryInfo startingDirectory)
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
                                        .Select(
                                            directory => new DirectoryNode(
                                                directory,
                                                startingDirectory))
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