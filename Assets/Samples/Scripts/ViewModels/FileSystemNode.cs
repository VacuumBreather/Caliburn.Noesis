namespace Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Caliburn.Noesis;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    /// <summary>
    ///     Base class for nodes representing parts of a file system.
    /// </summary>
    public abstract class FileSystemNode : PropertyChangedBase
    {
        #region Constants and Fields

        private bool isExpanded;
        private bool isSelected;
        private string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSystemNode" /> class.
        /// </summary>
        /// <param name="startingDirectory">The starting directory.</param>
        /// <param name="name">(Optional) The name of this node.</param>
        protected FileSystemNode(DirectoryInfo startingDirectory, string name = "New Node")
        {
            Name = name;
            StartingDirectory = startingDirectory;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the children nodes.
        /// </summary>
        [UsedImplicitly]
        public BindableCollection<FileSystemNode> Children { get; } = new BindableCollection<FileSystemNode>();

        /// <summary>
        ///     Gets the the files under this node (if any).
        /// </summary>
        [UsedImplicitly]
        public ObservableCollection<FileInfo> Files { get; } = new ObservableCollection<FileInfo>();

        /// <summary>
        ///     Gets or sets a value indicating if this node is expanded.
        /// </summary>
        [UsedImplicitly]
        public bool IsExpanded
        {
            get => this.isExpanded;
            set
            {
                Set(ref this.isExpanded, value);

                foreach (var node in Children)
                {
                    if (this.isExpanded)
                    {
                        node.PopulateDirectories().Forget();
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating if this node has been populated with data.
        /// </summary>
        [UsedImplicitly]
        public bool IsPopulated { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating if this node is selected.
        /// </summary>
        [UsedImplicitly]
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                Set(ref this.isSelected, value);

                if (this.isSelected)
                {
                    PopulateFiles().Forget();
                }
                else
                {
                    Files.Clear();
                }
            }
        }

        /// <summary>
        ///     Gets the name of this node.
        /// </summary>
        [UsedImplicitly]
        public string Name
        {
            get => this.name;
            private set => Set(ref this.name, value);
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the starting directory of the tree.
        /// </summary>
        protected DirectoryInfo StartingDirectory { get; }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Override this to provide the children of this node.
        /// </summary>
        /// <param name="startingDirectory">The starting directory of the tree.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the child nodes.
        /// </returns>
        protected virtual async UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(DirectoryInfo startingDirectory)
        {
            return await UniTask.FromResult(Array.Empty<FileSystemNode>());
        }

        /// <summary>
        ///     Override this to provide the files under this node.
        /// </summary>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the files under this nodes.
        /// </returns>
        protected virtual async UniTask<IEnumerable<FileInfo>> GetFilesAsync()
        {
            return await UniTask.FromResult(Array.Empty<FileInfo>());
        }

        /// <summary>
        ///     Override this to expand the node if it is part of the path of the specified starting directory.
        /// </summary>
        /// <param name="startingDirectory">The starting directory.</param>
        protected virtual void PotentiallyExpand(DirectoryInfo startingDirectory)
        {
        }

        /// <summary>
        ///     Populates this node with its sub directories.
        /// </summary>
        protected async UniTask PopulateDirectories()
        {
            if (IsPopulated)
            {
                return;
            }

            await UniTask.SwitchToThreadPool();
            var children = await GetChildrenAsync(StartingDirectory);
            await UniTask.SwitchToMainThread();

            foreach (var child in children)
            {
                Children.Add(child);
            }

            PotentiallyExpand(StartingDirectory);

            IsPopulated = true;
        }

        #endregion

        #region Private Methods

        private async UniTask PopulateFiles()
        {
            await UniTask.SwitchToThreadPool();
            var files = await GetFilesAsync();
            await UniTask.SwitchToMainThread();

            foreach (var file in files)
            {
                Files.Add(file);
            }
        }

        #endregion
    }
}