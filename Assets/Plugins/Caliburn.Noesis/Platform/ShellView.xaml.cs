// <copyright file="ShellView.xaml.cs" company="VacuumBreather">
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
    ///     Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : Page
    {
        #region Constructors and Destructors

        public ShellView()
        {
            InitializeComponent();
        }

        #endregion

        #region Private Methods

#if NOESIS
        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Scripts/MapEditor/Views/ShellView.xaml");
        }
#endif

        #endregion
    }
}