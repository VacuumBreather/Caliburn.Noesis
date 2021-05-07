namespace Caliburn.Noesis
{
    using System;

    /// <summary>
    ///     Event arguments for the <see cref="IDeactivate.Deactivating" />
    ///     <see cref="IDeactivate.Deactivated" /> events.
    /// </summary>
    public class DeactivationEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>Indicates whether the sender was closed in addition to being deactivated.</summary>
        public bool WasClosed { get; set; }

        #endregion
    }
}