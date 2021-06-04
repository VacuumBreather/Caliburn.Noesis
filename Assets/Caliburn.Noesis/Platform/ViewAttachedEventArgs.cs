namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using EventArgs = System.EventArgs;
    using global::Noesis;

#else
    using System.Windows;
#endif

    /// <summary>Event arguments for the <see cref="IViewAware.ViewAttached" /> event.</summary>
    [PublicAPI]
    public class ViewAttachedEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>The ID of the context in which the view appears.</summary>
        public Guid Context { get; set; }

        /// <summary>The attached view.</summary>
        public UIElement View { get; set; }

        #endregion
    }
}