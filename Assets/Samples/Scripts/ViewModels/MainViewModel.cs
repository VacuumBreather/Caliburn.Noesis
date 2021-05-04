namespace Samples.ViewModels
{
    #region Using Directives

    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Caliburn.Noesis;
    using Caliburn.Noesis.Extensions;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

    #endregion

    /// <summary>The main view-model of the sample UI.</summary>
    /// <seealso cref="Screen" />
    public class MainViewModel : Screen
    {
        #region Constants and Fields

        private readonly AsyncGuard asyncGuard = new AsyncGuard();
        private SafeCancellationTokenSource cancellationTokenSource;
        private string fileContent;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        public MainViewModel(IWindowManager windowManager)
        {
            WindowManager = windowManager;
            OpenDialogCommand =
                new AsyncRelayCommand(OpenDialogAsync, () => IsActive).RaiseWith(this);
            CancelCommand = new RelayCommand(CancelReadingFile, () => this.asyncGuard.IsOngoing)
                .RaiseWith(this.asyncGuard);
        }

        #endregion

        #region Public Properties

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

        private FileDialogViewModel FileDialog { get; } = new FileDialogViewModel();

        private IWindowManager WindowManager { get; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override UniTask OnDeactivateAsync(bool close,
                                                     CancellationToken cancellationToken)
        {
            if (this.cancellationTokenSource is { IsDisposed: false })
            {
                this.cancellationTokenSource.Cancel();
            }

            return UniTask.CompletedTask;
        }

        #endregion

        #region Private Methods

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
                    cancelled = await ReadFileAsync();

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

        private async Task<bool> ReadFileAsync()
        {
            using var token = this.asyncGuard.Token;
            using var tokenSource =
                this.cancellationTokenSource = new SafeCancellationTokenSource();

            var cancellationToken = this.cancellationTokenSource.Token;
            var cancelled = cancellationToken.IsCancellationRequested;

            using var streamReader = File.OpenText(FileDialog.SelectedFile.FullName);

            FileContent = string.Empty;

            while (!(streamReader.EndOfStream || cancelled))
            {
                FileContent += $"{await streamReader.ReadLineAsync()}\n";

                cancelled = await UniTask.Delay(100, cancellationToken: cancellationToken)
                                         .SuppressCancellationThrow();
                cancelled = cancelled || cancellationToken.IsCancellationRequested;
            }

            if (!cancelled)
            {
                FileContent += "\n--- Done ---";
            }

            return cancelled;
        }

        #endregion
    }
}