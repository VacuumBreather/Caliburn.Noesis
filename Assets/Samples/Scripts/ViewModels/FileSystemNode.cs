// <copyright file="FileSystemNode.cs" company="VacuumBreather">
//      Copyright © 2021 VacuumBreather. All rights reserved.
// </copyright>

namespace Caliburn.Noesis.Samples.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using Cysharp.Threading.Tasks;

    public class FileSystemNode : PropertyChangedBase
    {
        #region Constants and Fields

        private bool isExpanded;
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

        public ObservableCollection<FileInfo> Files { get; } = new ObservableCollection<FileInfo>();

        #region Public Properties

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

        public BindableCollection<FileSystemNode> Children { get; } = new BindableCollection<FileSystemNode>();

        #endregion

        #region Protected Methods

        public bool IsPopulated { get; private set; }
        private bool isSelected;

        protected async UniTask PopulateFiles()
        {
            IEnumerable<FileInfo> files;

#if NOESIS
            await UniTask.SwitchToThreadPool();
            files = await GetFilesAsync();
            await UniTask.SwitchToMainThread();
#else
            await using (UniTask.ReturnToCurrentSynchronizationContext())
            {
                await UniTask.SwitchToThreadPool();
                files = await GetFilesAsync();
            }
#endif

            foreach (var file in files)
            {
                Files.Add(file);
            }
        }

        protected async UniTask PopulateDirectories()
        {
            if (IsPopulated)
            {
                return;
            }

#if NOESIS
            await UniTask.SwitchToThreadPool();

            var children = await GetChildrenAsync(StartingDirectory);
            
            await UniTask.SwitchToMainThread();
#else
            IEnumerable<FileSystemNode> children;

            await using (UniTask.ReturnToCurrentSynchronizationContext())
            {
                await UniTask.SwitchToThreadPool();
                children = await GetChildrenAsync(StartingDirectory);
            }
#endif

            foreach (var child in children)
            {
                Children.Add(child);
            }

            PotentiallyExpand(StartingDirectory);

            IsPopulated = true;
        }

        protected virtual void PotentiallyExpand(DirectoryInfo startingDirectory)
        {
        }

        protected virtual async UniTask<IEnumerable<FileSystemNode>> GetChildrenAsync(DirectoryInfo startingDirectory)
        {
            return await UniTask.FromResult(Array.Empty<FileSystemNode>());
        }

        protected virtual async UniTask<IEnumerable<FileInfo>> GetFilesAsync()
        {
            return await UniTask.FromResult(Array.Empty<FileInfo>());
        }

        #endregion
    }
}