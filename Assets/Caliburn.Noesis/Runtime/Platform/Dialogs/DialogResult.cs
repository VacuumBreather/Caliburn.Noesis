namespace Caliburn.Noesis
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a dialog.
    /// </summary>
    public enum DialogResult
    {
        /// <summary>
        /// Nothing is returned from the dialog. This means that the modal dialog continues running.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dialog return value is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// The dialog return value is OK (usually sent from a button labeled OK).
        /// </summary>
        Ok = 2,

        /// <summary>
        /// The dialog return value is Yes (usually sent from a button labeled Yes).
        /// </summary>
        Yes = 3,

        /// <summary>
        /// The dialog return value is No (usually sent from a button labeled No).
        /// </summary>
        No = 4,
    }
}
