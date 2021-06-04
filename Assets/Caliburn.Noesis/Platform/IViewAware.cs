namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;
    using ViewAttachedEventHandler = System.EventHandler<ViewAttachedEventArgs>;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
#endif

    /// <summary>Defines a view-model type which is aware of its view.</summary>
    [PublicAPI]
    public interface IViewAware
    {
        /// <summary>Raised when a view is attached.</summary>
        event ViewAttachedEventHandler ViewAttached;

        /// <summary>Attaches a view to this instance.</summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The ID of the context in which the view appears.</param>
        void AttachView(UIElement view, Guid context);

        /// <summary>Gets a view previously attached to this instance.</summary>
        /// <param name="context">The ID of the context in which the view appears.</param>
        /// <returns>The attached view, or <c>null</c> if no view has been attached for the specified context.</returns>
        UIElement GetView(Guid context);
    }
}