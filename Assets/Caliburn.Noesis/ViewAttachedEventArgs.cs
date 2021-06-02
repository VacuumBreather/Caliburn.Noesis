namespace Caliburn.Noesis
{
    using System;

    /// <summary>Event arguments for the <see cref="IViewAware.ViewAttached" /> event.</summary>
    public class ViewAttachedEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>The attached view.</summary>
        public object View { get; set; }

        #endregion
    }
}