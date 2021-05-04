﻿namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using global::Noesis;

    #endregion

    /// <summary>Contains attached dependency properties used when closing a dialog.</summary>
    public static class CloseDialog
    {
        #region Constants and Fields

        /// <summary>
        ///     Result property. This is an attached property. CloseDialog defines the Result, so that it
        ///     can be set on any <see cref="Button" /> that is used to close a dialog with that result.
        /// </summary>
        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(ResultProperty)),
                typeof(DialogResult),
                typeof(CloseDialog),
                new PropertyMetadata(default(DialogResult), OnResultChanged));

        #endregion

        #region Public Methods

        /// <summary>Gets the result to close a dialog with.</summary>
        /// <param name="button">The button which sets the dialog result.</param>
        /// <returns>The dialog result the dialog will be closed with.</returns>
        public static DialogResult GetResult(Button button)
        {
            return (DialogResult)button.GetValue(ResultProperty);
        }

        /// <summary>Sets the result to close a dialog with.</summary>
        /// <param name="button">The button which sets the dialog result.</param>
        /// <param name="result">The dialog result to close the dialog with.</param>
        public static void SetResult(Button button, DialogResult result)
        {
            button.SetValue(ResultProperty, result);
        }

        #endregion

        #region Private Methods

        private static void OnResultChanged(DependencyObject d,
                                            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Button button))
            {
                return;
            }

            button.CommandParameter = e.NewValue switch
                {
                    DialogResult.Cancel => null,
                    DialogResult.Ok => true,
                    DialogResult.Yes => true,
                    DialogResult.No => false,
                    _ => throw new ArgumentOutOfRangeException()
                };

            var commandBinding =
                new Binding(nameof(DialogScreen.CloseDialogCommand)) { Mode = BindingMode.OneWay };
            button.SetBinding(ButtonBase.CommandProperty, commandBinding);
        }

        #endregion
    }
}