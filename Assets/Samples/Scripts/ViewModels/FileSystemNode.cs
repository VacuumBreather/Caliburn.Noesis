namespace Caliburn.Noesis.Samples.ViewModels
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Cysharp.Threading.Tasks;

    #endregion

    public class FileSystemNode : PropertyChangedBase
    {
        #region Constants and Fields

        private bool isExpanded;
        private bool isSelected;
        private string name;

        #endregion

        #region Constructors and Destructors

        /// <inheritdoc />
        public FileSystemNode(DirectoryInfo startingDirectory, string name = "New Node")
        {
            Name = name;
            StartingDirectory = startingDirectory;
        }

        #endregion

        #region Public Properties

        public BindableCollection<FileSystemNode> Children { get; } = new BindableCollection<FileSystemNode>();

        public ObservableCollection<FileInfo> Files { get; } = new ObservableCollection<FileInfo>();

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

        public bool IsPopulated { get; private set; }

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

        public string Name
        {
            get => this.name;
            private set => Set(ref this.name, value);
        }

        public DirectoryInfo StartingDirectory { get; }

        #endregion

        #region Protected Methods

        protected virtual async UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(DirectoryInfo startingDirectory)
        {
            return await UniTask.FromResult(Array.Empty<FileSystemNode>());
        }

        protected virtual async UniTask<IEnumerable<FileInfo>> GetFilesAsync()
        {
            return await UniTask.FromResult(Array.Empty<FileInfo>());
        }

        protected virtual void PotentiallyExpand(DirectoryInfo startingDirectory)
        {
        }

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

        protected async UniTask PopulateFiles()
        {
            IEnumerable<FileInfo> files;

            await UniTask.SwitchToThreadPool();
            files = await GetFilesAsync();
            await UniTask.SwitchToMainThread();

            foreach (var file in files)
            {
                Files.Add(file);
            }
        }

        #endregion
    }
}