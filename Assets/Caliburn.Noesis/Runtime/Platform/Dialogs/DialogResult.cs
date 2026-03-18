namespace Caliburn.Noesis
{
    /// <summary>
    ///     Specifies identifiers to indicate the return value of a dialog.
    /// </summary>
    public enum DialogResult
    {
        /// <summary>
        /// The dialog returns no result.
        /// </summary>
        None = 0,

        /// <summary>
        /// The result value of the message box is OK (usually sent from a button labeled OK).
        /// </summary>
        Ok = 1,

        /// <summary>
        /// The result value of the message box is Cancel (usually sent from a button labeled Cancel).
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// The result value of the message box is Abort (usually sent from a button labeled Abort).
        /// </summary>
        Abort = 3,

        /// <summary>
        /// The result value of the message box is Retry (usually sent from a button labeled Retry).
        /// </summary>
        Retry = 4,

        /// <summary>
        /// The result value of the message box is Ignore (usually sent from a button labeled Ignore).
        /// </summary>
        Ignore = 5,

        /// <summary>
        /// The dialog return value is Yes (usually sent from a button labeled Yes).
        /// </summary>
        Yes = 6,

        /// <summary>
        /// The dialog return value is No (usually sent from a button labeled No).
        /// </summary>
        No = 7,

        /// <summary>
        /// The result value of the message box is TryAgain (usually sent from a button labeled Try Again).
        /// </summary>
        TryAgain = 8,

        /// <summary>
        /// The result value of the message box is Continue (usually sent from a button labeled Continue).
        /// </summary>
        Continue = 9
    }
}
