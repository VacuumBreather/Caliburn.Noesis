// <copyright file="MainView.xaml.cs" company="VacuumBreather">
//      Copyright © 2021 VacuumBreather. All rights reserved.
// </copyright>

namespace VacuumBreather.Montreal.MapEditor.Views
{
    #region Using Directives

#if NOESIS
    using Noesis;

#else
    using System.Windows.Controls;
#endif

    #endregion

    /// <summary>
    ///     Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        #region Constructors and Destructors

        public MainView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Scripts/MapEditor/Views/MainView.xaml");
        }
#endif

        #endregion
    }
}