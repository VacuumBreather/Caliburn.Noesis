namespace Caliburn.Noesis.Extensions
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Media;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    /// <summary>Provides extension methods for the <see cref="DependencyObject" /> type.</summary>
    public static class DependencyObjectExtensions
    {
        #region Public Methods

        /// <summary>Finds the first visual child of the specified type.</summary>
        /// <typeparam name="T">The type of the child to look for.</typeparam>
        /// <param name="parent">The parent object.</param>
        /// <returns>The first visual child of the specified type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent" /> is <c>null</c>.</exception>
        public static T FindVisualChild<T>([NotNull] this DependencyObject parent)
            where T : DependencyObject
        {
            return FindVisualChildren<T>(parent).FirstOrDefault();
        }

        /// <summary>Finds all visual children of the specified type.</summary>
        /// <typeparam name="T">The type of children to look for.</typeparam>
        /// <param name="parent">The parent object.</param>
        /// <returns>All visual children of the specified type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent" /> is <c>null</c>.</exception>
        public static IEnumerable<T> FindVisualChildren<T>([NotNull] this DependencyObject parent)
            where T : DependencyObject
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var queue = new Queue<DependencyObject>(
                new[]
                    {
                        parent
                    });

            while (queue.Any())
            {
                var reference = queue.Dequeue();
                var count = VisualTreeHelper.GetChildrenCount(reference);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);

                    if (child is T children)
                    {
                        yield return children;
                    }

                    queue.Enqueue(child);
                }
            }
        }

        #endregion
    }
}