namespace Caliburn.Noesis
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
#endif

    /// <summary>
    ///     A base implementation of <see cref="IViewAware" /> which is capable of caching views by
    ///     context.
    /// </summary>
    [PublicAPI]
    public class ViewAware : PropertyChangedBase, IViewAware
    {
        #region Constants and Fields

        private readonly IDictionary<Guid, UIElement> viewCache = new Dictionary<Guid, UIElement>();

        #endregion

        #region IViewAware Implementation

        /// <inheritdoc />
        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        /// <inheritdoc />
        public void AttachView(UIElement view, Guid context)
        {
            this.viewCache[context] = view;
            OnViewAttached(view, context);
        }

        /// <inheritdoc />
        public UIElement GetView(Guid context)
        {
            return this.viewCache.TryGetValue(context, out var view) ? view : null;
        }

        #endregion

        #region Protected Methods

        /// <summary>Called when a view is attached.</summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The ID of the context in which the view appears.</param>
        protected virtual void OnViewAttached(UIElement view, Guid context)
        {
            ViewAttached?.Invoke(
                this,
                new ViewAttachedEventArgs { View = view, Context = context });
        }

        #endregion
    }
}