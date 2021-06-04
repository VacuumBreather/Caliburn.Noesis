namespace Caliburn.Noesis.Samples.FileExplorer.ViewModels
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using JetBrains.Annotations;

    /// <summary>The main view-model of the sample.</summary>
    /// <seealso cref="Screen" />
    [PublicAPI]
    public class MainViewModel : Screen
    {
        #region Constants and Fields

        private readonly AsyncGuard asyncGuard = new AsyncGuard();
        private SafeCancellationTokenSource cancellationTokenSource;
        private string fileContent;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="fileDialog">The file dialog.</param>
        /// <param name="createSampleWindow">A function to create sample windows.</param>
        public MainViewModel(IWindowManager windowManager,
                             FileDialogViewModel fileDialog,
                             Func<SampleWindowViewModel> createSampleWindow)
        {
            WindowManager = windowManager;
            FileDialog = fileDialog;
            CreateSampleWindow = createSampleWindow;

            OpenDialogCommand = new AsyncRelayCommand(OpenDialogAsync, () => IsActive).RaiseWith(this);
            CancelCommand =
                new RelayCommand(CancelReadingFile, () => this.asyncGuard.IsOngoing).RaiseWith(this.asyncGuard);
            AddWindowCommand = new AsyncRelayCommand(AddWindow, () => IsActive).RaiseWith(this);
        }

        #endregion

        #region Public Properties

        /// <summary>The command to add another window.</summary>
        [UsedImplicitly]
        public IRaisingCommand AddWindowCommand { get; }

        /// <summary>The command to cancel reading the opened file.</summary>
        [UsedImplicitly]
        public IRaisingCommand CancelCommand { get; }

        /// <summary>Gets the file content.</summary>
        public string FileContent
        {
            get => this.fileContent;
            private set => Set(ref this.fileContent, value);
        }

        /// <summary>The command to open the file dialog.</summary>
        [UsedImplicitly]
        public IRaisingCommand OpenDialogCommand { get; }

        #endregion

        #region Private Properties

        private Func<SampleWindowViewModel> CreateSampleWindow { get; }

        private FileDialogViewModel FileDialog { get; }

        private IWindowManager WindowManager { get; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override UniTask OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (this.cancellationTokenSource is { IsDisposed: false })
            {
                this.cancellationTokenSource.Cancel();
            }

            return UniTask.CompletedTask;
        }

        #endregion

        #region Private Methods

        private async UniTask AddWindow()
        {
            await WindowManager.ShowWindowAsync(CreateSampleWindow());
        }

        private void CancelReadingFile()
        {
            if (this.cancellationTokenSource is { IsDisposed: false })
            {
                this.cancellationTokenSource.Cancel();
            }
        }

        private async UniTask OpenDialogAsync()
        {
            var result = await WindowManager.ShowDialogAsync(FileDialog);
            var cancelled = false;

            switch (result)
            {
                case true when FileDialog.SelectedFile.Exists:
                    cancelled = await ReadFileAsync(FileDialog.SelectedFile);

                    break;
                case true when !FileDialog.SelectedFile.Exists:
                    FileContent = "Invalid file!";

                    break;
                default:
                    FileContent = "No file opened.";

                    break;
            }

            if (cancelled)
            {
                FileContent += "\n--- Cancelled ---";
            }
        }

        private async Task<bool> ReadFileAsync(FileInfo file)
        {
            using var _ = this.asyncGuard.GetToken();
            using var tokenSource = this.cancellationTokenSource = new SafeCancellationTokenSource();

            var cancellationToken = this.cancellationTokenSource.Token;
            var cancelled = cancellationToken.IsCancellationRequested;

            using var streamReader = File.OpenText(file.FullName);

            FileContent = string.Empty;
            var stringBuilder = new StringBuilder();

#if UNITY_5_5_OR_NEWER
            while (!(streamReader.EndOfStream || cancelled))
            {
                stringBuilder.AppendLine(await streamReader.ReadLineAsync());
                FileContent = stringBuilder.ToString();

                cancelled = await UniTask.Delay(10, cancellationToken: cancellationToken).SuppressCancellationThrow();
                cancelled = cancelled || cancellationToken.IsCancellationRequested;
            }
#else
            try
            {
                while (!(streamReader.EndOfStream || cancelled))
                {
                    stringBuilder.AppendLine(await streamReader.ReadLineAsync());
                    FileContent = stringBuilder.ToString();

                    await Task.Delay(10, cancellationToken);
                    cancelled = cancellationToken.IsCancellationRequested;
                }
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }
#endif

            if (!cancelled)
            {
                FileContent += "\n--- Done ---";
            }

            return cancelled;
        }

        #endregion
    }
}