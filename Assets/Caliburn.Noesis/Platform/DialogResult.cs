namespace Caliburn.Noesis
{
    /// <summary>Specifies identifiers to indicate the return value of a dialog.</summary>
    public enum DialogResult
    {
        /// <summary>Nothing is returned from the dialog. This means that the modal dialog continues running.</summary>
        None = 0,

        /// <summary>The dialog return value is Cancel (usually sent from a button labeled Cancel).</summary>
        Cancel,

        /// <summary>The dialog return value is OK (usually sent from a button labeled OK).</summary>
        Ok,

        /// <summary>The dialog return value is Yes (usually sent from a button labeled Yes).</summary>
        Yes,

        /// <summary>The dialog return value is No (usually sent from a button labeled No).</summary>
        No
    }
}