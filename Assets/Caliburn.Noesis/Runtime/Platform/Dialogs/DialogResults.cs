using System;

namespace Caliburn.Noesis
{
    /// <summary>
    /// Specifies identifiers to indicate the possible return values of a dialog.
    /// </summary>
    [Flags]
    public enum DialogResults
    {
        /// <summary>No possible return value is defined.</summary>
        None = 0,

        /// <summary>The dialog can return Cancel.</summary>
        Cancel = 0b0001,

        /// <summary>The dialog can return Ok.</summary>
        Ok = 0b0010,

        /// <summary>The dialog can return either Ok or Cancel.</summary>
        OkCancel = Ok | Cancel,

        /// <summary>The dialog can return Yes.</summary>
        Yes = 0b0100,

        /// <summary>The dialog can return No.</summary>
        No = 0b1000,

        /// <summary>The dialog can return either Yes or No.</summary>
        YesNo = Yes | No,

        /// <summary>The dialog can return either Yes, No or Cancel.</summary>
        YesNoCancel = Yes | No | Cancel,
    }
}
