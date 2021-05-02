// <copyright file="ShellView.xaml.cs" company="VacuumBreather">
//      Copyright © 2021 VacuumBreather. All rights reserved.
// </copyright>

namespace Caliburn.Noesis
{
    using global::Noesis;

    #region Using Directives

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

        private void InitializeComponent()
        {
            GUI.LoadComponent(this, "Assets/Caliburn.Noesis/Platform/ShellView.xaml");
        }

        #endregion
    }
}