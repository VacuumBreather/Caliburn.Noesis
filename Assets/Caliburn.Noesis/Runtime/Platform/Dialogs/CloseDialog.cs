using Noesis;

namespace Caliburn.Noesis
{
    /// <summary>Contains attached dependency properties used when closing a dialog.</summary>
    public static class CloseDialog
    {
        private const string CloseDialogAsync = nameof(DialogScreen.CloseDialogAsync);

        private static readonly string Click = nameof(ButtonBase.ClickEvent).Replace("Event", string.Empty);

        /// <summary>
        ///     Result property. This is an attached property. CloseDialog defines the Result, so that it can be set on any
        ///     <see cref="Button"/> that is used to close a dialog with that result.
        /// </summary>
        public static readonly DependencyProperty ResultProperty = DependencyProperty.RegisterAttached(
            "Result",
            typeof(DialogResult),
            typeof(CloseDialog),
            new PropertyMetadata(default(DialogResult), OnResultChanged));

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

        private static void OnResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Button button)
            {
                return;
            }

            var result = ((DialogResult)e.NewValue).ToString().ToUpper();
            
            Message.SetAttach(button, $"[Event {Click}] = [Action {CloseDialogAsync}('{result}')]");
        }
    }
}
