namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System.Collections.Specialized;
    using Extensions;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Controls;
#endif

    /// <summary>
    ///     Provides an attached property which handles scrolling a <see cref="ListBox" /> back to the
    ///     top if its <see cref="ListBox.ItemsSource" /> has been changed.
    /// </summary>
    public static class ScrollToTopBehavior
    {
        #region Constants and Fields

        /// <summary>The ScrollToTop property.</summary>
        public static readonly DependencyProperty ScrollToTopProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(ScrollToTopProperty)),
                typeof(bool),
                typeof(ScrollToTopBehavior),
                new PropertyMetadata(default(bool), OnScrollToTopChanged));

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets a value indicating whether the target <see cref="ListBox" /> will scroll to the top
        ///     if its <see cref="ListBox.ItemsSource" /> has been changed.
        /// </summary>
        /// <param name="listBox">The <see cref="ListBox" /> which might scroll.</param>
        /// <returns>
        ///     <c>true</c> the target <see cref="ListBox" /> will scroll to the top if its
        ///     <see cref="ListBox.ItemsSource" /> has been changed; otherwise, <c>false</c>.
        /// </returns>
        public static bool GetScrollToTop(ListBox listBox)
        {
            return (bool)listBox.GetValue(ScrollToTopProperty);
        }

        /// <summary>
        ///     Sets a value indicating whether the target <see cref="ListBox" /> should scroll to the top
        ///     if its <see cref="ListBox.ItemsSource" /> has been changed.
        /// </summary>
        /// <param name="listBox">The <see cref="ListBox" /> which should scroll.</param>
        /// <param name="value">
        ///     if set to <c>true</c> the target <see cref="ListBox" /> will scroll to the top
        ///     if its <see cref="ListBox.ItemsSource" /> has been changed.
        /// </param>
        public static void SetScrollToTop(ListBox listBox, bool value)
        {
            listBox.SetValue(ScrollToTopProperty, value);
        }

        #endregion

        #region Private Methods

        private static void OnScrollToTopChanged(DependencyObject d,
                                                 DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ListBox { Items: INotifyCollectionChanged notifyingCollection } listBox))
            {
                return;
            }

            void OnListBoxCollectionChanged(object s, NotifyCollectionChangedEventArgs args)
            {
                if (listBox.FindVisualChild<ScrollViewer>() is { } scrollViewer)
                {
                    scrollViewer.ScrollToTop();
                }
            }

            if (e.NewValue is true)
            {
                notifyingCollection.CollectionChanged += OnListBoxCollectionChanged;
            }
            else
            {
                notifyingCollection.CollectionChanged -= OnListBoxCollectionChanged;
            }
        }

        #endregion
    }
}