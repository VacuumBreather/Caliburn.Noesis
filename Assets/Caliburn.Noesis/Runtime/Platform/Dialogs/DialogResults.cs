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

        /// <summary>The dialog can return Ok.</summary>
        Ok = 0b0000_0000_0001,

        /// <summary>The dialog can return Cancel.</summary>
        Cancel = 0b0000_0000_0010,

        /// <summary>The dialog can return Abort.</summary>
        Abort = 0b0000_0000_0100,

        /// <summary>The dialog can return Retry.</summary>
        Retry = 0b0000_0000_1000,

        /// <summary>The dialog can return Ignore.</summary>
        Ignore = 0b0000_0001_0000,

        /// <summary>The dialog can return Yes.</summary>
        Yes = 0b0000_0010_0000,

        /// <summary>The dialog can return No.</summary>
        No = 0b0000_0100_0000,

        /// <summary>The dialog can return Ignore.</summary>
        TryAgain = 0b0000_1000_0000,

        /// <summary>The dialog can return Ignore.</summary>
        Continue = 0b0001_0000_0000,

        /// <summary>The dialog can return either Ok or Cancel.</summary>
        OkCancel = Ok | Cancel,

        /// <summary>The dialog can return either Yes or No.</summary>
        YesNo = Yes | No,

        /// <summary>The dialog can return either Yes, No, or Cancel.</summary>
        YesNoCancel = Yes | No | Cancel,

        /// <summary>The dialog can return either Retry or Abort.</summary>
        RetryAbort = Retry | Abort,

        /// <summary>The dialog can return either Retry, Abort, or Continue.</summary>
        RetryAbortContinue = Retry | Abort | Continue,

        /// <summary>The dialog can return either TryAgain or Abort.</summary>
        TryAgainAbort = TryAgain | Abort,

        /// <summary>The dialog can return either TryAgain, Abort, or Ignore.</summary>
        TryAgainAbortIgnore = TryAgain | Abort | Ignore,
    }
}
