namespace Caliburn.Noesis
{
    using System;

    /// <summary>Defines a view-model type which is aware of its view.</summary>
    public interface IViewAware
    {
        /// <summary>Raised when a view is attached.</summary>
        event EventHandler<ViewAttachedEventArgs> ViewAttached;

        /// <summary>Gets a view previously attached to this instance.</summary>
        /// <value>The view attached to this instance.</value>
        object View { get; }

        /// <summary>Attaches a view to this instance.</summary>
        /// <param name="view">The view.</param>
        void AttachView(object view);
    }
}