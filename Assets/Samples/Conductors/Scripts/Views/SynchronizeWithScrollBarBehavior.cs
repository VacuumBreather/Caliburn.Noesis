namespace Caliburn.Noesis.Samples.Conductors.Views
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Interactivity;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
#endif
    using System.Linq;
    using Extensions;

    /// <summary>
    ///     Synchronizes the attached <see cref="ScrollViewer" /> with a specified external
    ///     <see cref="ScrollBar" />.
    /// </summary>
    public class SynchronizeWithScrollBarBehavior : Behavior<ScrollViewer>
    {
        #region Constants and Fields

        /// <summary>The ScrollBar property.</summary>
        public static readonly DependencyProperty ScrollBarProperty = DependencyProperty.Register(
            nameof(ScrollBar),
            typeof(ScrollBar),
            typeof(SynchronizeWithScrollBarBehavior),
            new PropertyMetadata(default(ScrollBar), OnScrollBarChanged));

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the <see cref="ScrollBar" /> to synchronize with.</summary>
        /// <value>The<see cref="ScrollBar" />.</value>
        public ScrollBar ScrollBar
        {
            get => (ScrollBar)GetValue(ScrollBarProperty);
            set => SetValue(ScrollBarProperty, value);
        }

        #endregion

        #region Private Properties

        private ScrollBar ScrollViewerScrollBar { get; set; }

        #endregion

        #region Protected Methods

        /// <inheritdoc />
        protected override void OnAttached()
        {
            if (ScrollBar != null)
            {
                AttachTo(ScrollBar);
            }
        }

        /// <inheritdoc />
        protected override void OnDetaching()
        {
            if (ScrollBar != null)
            {
                DetachFrom(ScrollBar);
            }
        }

        #endregion

        #region Event Handlers

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            ScrollViewerScrollBar = AssociatedObject.FindVisualChildren<ScrollBar>()
                                                    .First(
                                                        bar => bar.Orientation ==
                                                               Orientation.Vertical);

            AssociatedObject.SizeChanged += OnAssociatedObjectSizeChanged;
            UpdateVisibility(ScrollBar);
        }

        private void OnAssociatedObjectScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollViewerScrollBar != null)
            {
                ScrollBar.Minimum = ScrollViewerScrollBar.Minimum;
                ScrollBar.Maximum = ScrollViewerScrollBar.Maximum;
            }

            ScrollBar.Value = AssociatedObject.VerticalOffset;
        }

        private void OnAssociatedObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVisibility(ScrollBar);
        }

        private void OnScrollBarScroll(object sender, ScrollEventArgs e)
        {
            var scrollBar = (ScrollBar)sender;

            if (ScrollViewerScrollBar != null)
            {
                scrollBar.Minimum = ScrollViewerScrollBar.Minimum;
                scrollBar.Maximum = ScrollViewerScrollBar.Maximum;
            }

            scrollBar.Scroll -= OnScrollBarScroll;
            AssociatedObject.ScrollToVerticalOffset(scrollBar.Value);
            scrollBar.Scroll += OnScrollBarScroll;
        }

        #endregion

        #region Private Methods

        private static void OnScrollBarChanged(DependencyObject d,
                                               DependencyPropertyChangedEventArgs e)
        {
            ((SynchronizeWithScrollBarBehavior)d).OnScrollBarChanged(
                (ScrollBar)e.OldValue,
                (ScrollBar)e.NewValue);
        }

        private void AttachTo(ScrollBar scrollBar)
        {
            scrollBar.Scroll += OnScrollBarScroll;
            AssociatedObject.ScrollChanged += OnAssociatedObjectScrollChanged;
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            UpdateVisibility(scrollBar);
        }

        private void DetachFrom(ScrollBar scrollBar)
        {
            scrollBar.Scroll -= OnScrollBarScroll;
            AssociatedObject.ScrollChanged -= OnAssociatedObjectScrollChanged;
        }

        private void OnScrollBarChanged(ScrollBar oldScrollBar, ScrollBar newScrollBar)
        {
            if (oldScrollBar != null)
            {
                DetachFrom(oldScrollBar);
            }

            if (newScrollBar != null)
            {
                AttachTo(newScrollBar);
            }
        }

        private void UpdateVisibility(ScrollBar scrollBar)
        {
            if (scrollBar != null)
            {
                var canScroll = AssociatedObject.ExtentHeight > AssociatedObject.ViewportHeight;

                scrollBar.Visibility = canScroll ? Visibility.Visible : Visibility.Hidden;
            }
        }

        #endregion
    }
}