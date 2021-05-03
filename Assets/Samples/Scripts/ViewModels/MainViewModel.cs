namespace Samples.ViewModels
{
    #region Using Directives

    using System.Windows.Input;
    using Caliburn.Noesis;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;

#if UNITY_5_3_OR_NEWER

#else
    using System;

#endif

    #endregion

    /// <summary>
    ///     The main view-model of the sample UI.
    /// </summary>
    /// <seealso cref="Screen" />
    public class MainViewModel : Screen
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel(IWindowManager windowManager)
        {
            WindowManager = windowManager;
            OpenDialogCommand = new RelayCommand(() => OpenDialog().Forget());
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The command to open the file dialog.
        /// </summary>
        [UsedImplicitly]
        public ICommand OpenDialogCommand { get; }

        #endregion

        #region Private Properties

        private FileDialogViewModel FileDialog { get; } = new FileDialogViewModel();

        private IWindowManager WindowManager { get; }

        #endregion

        #region Private Methods

        private async UniTask OpenDialog()
        {
            var result = await WindowManager.ShowDialogAsync(FileDialog);

            Logger.Info(
                result == true ? $"File selected: {FileDialog.FileInfo?.FullName ?? "null"}" : "Operation canceled");
        }

        #endregion
    }
}