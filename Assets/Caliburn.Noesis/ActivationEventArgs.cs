namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;

    /// <summary>Event arguments for the <see cref="IActivate.Activated" /> event.</summary>
    [PublicAPI]
    public class ActivationEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>Indicates whether the sender was initialized in addition to being activated.</summary>
        public bool WasInitialized { get; set; }

        #endregion
    }
}