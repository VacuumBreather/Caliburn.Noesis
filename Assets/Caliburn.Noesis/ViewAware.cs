namespace Caliburn.Noesis
{
    using System;
    using JetBrains.Annotations;

    /// <summary>
    ///     A base implementation of <see cref="IViewAware" /> which is capable of caching views by
    ///     context.
    /// </summary>
    [PublicAPI]
    public class ViewAware : PropertyChangedBase, IViewAware
    {
        #region IViewAware Implementation

        /// <inheritdoc />
        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        /// <summary>Gets a view previously attached to this instance.</summary>
        /// <returns>The view.</returns>
        public object View { get; private set; }

        /// <inheritdoc />
        void IViewAware.AttachView(object view)
        {
            View = view;

            OnViewAttached(View);
        }

        #endregion

        #region Protected Methods

        /// <summary>Called when a view is attached.</summary>
        /// <param name="view">The view.</param>
        protected virtual void OnViewAttached(object view)
        {
            ViewAttached?.Invoke(this, new ViewAttachedEventArgs { View = view });
        }

        #endregion
    }
}