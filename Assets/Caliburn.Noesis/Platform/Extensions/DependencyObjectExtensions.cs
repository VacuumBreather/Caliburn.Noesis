namespace Caliburn.Noesis.Extensions
{
    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Media;
#endif

    /// <summary>Provides extension methods for the <see cref="DependencyObject" /> type.</summary>
    public static class DependencyObjectExtensions
    {
        #region Public Methods

        /// <summary>Goes up the visual tree, looking for an ancestor of the specified <see cref="Type" />.</summary>
        /// <typeparam name="T">The <see cref="Type" /> to look for.</typeparam>
        /// <param name="child">The starting point.</param>
        /// <returns>The visual ancestor, if any, of the specified starting point and <see cref="Type" />.</returns>
        public static T FindVisualAncestor<T>(this DependencyObject child)
            where T : DependencyObject
        {
            if (child == null)
            {
                return null;
            }

            do
            {
                child = VisualTreeHelper.GetParent(child);
            }
            while ((child != null) && !(child is T));

            return child as T;
        }

        #endregion
    }
}