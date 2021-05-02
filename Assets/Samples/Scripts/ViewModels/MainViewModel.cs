// <copyright file="MainViewModel.cs" company="VacuumBreather">
//      Copyright © 2021 VacuumBreather. All rights reserved.
// </copyright>

namespace Caliburn.Noesis.Samples.ViewModels
{
    #region Using Directives

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Input;
    using Cysharp.Threading.Tasks;
    using JetBrains.Annotations;
    using UnityEngine;
    using Screen = Caliburn.Noesis.Screen;
#if UNITY_5_3_OR_NEWER

#else
    using System;

#endif

    #endregion

    /// <summary>
    ///     The main view model.
    /// </summary>
    /// <seealso cref="PropertyChangedBase" />
    public class MainViewModel : Screen
    {
        private IWindowManager WindowManager { get; }

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScenarioLoader" /> class.
        /// </summary>
        public MainViewModel(IWindowManager windowManager)
        {
            WindowManager = windowManager;
            OpenDialogCommand = new RelayCommand(() => OpenDialog().Forget());
        }

        #endregion

        #region Public Properties

        [UsedImplicitly]
        public ICommand OpenDialogCommand { get; }

        #endregion

        #region Private Properties

        private FileDialogViewModel FileDialog { get; } = new FileDialogViewModel();

        #endregion

        #region Private Methods

        private async UniTask OpenDialog()
        {
            var fileDialog = FileDialog;
            
            var result = await WindowManager.ShowDialogAsync(FileDialog);

            Logger.Info(result == true ? $"File selected: {fileDialog.FileInfo?.FullName ?? "null"}" : "Operation canceled");
        }

        #endregion
    }
}