using System;
#if UNITY_5_5_OR_NEWER
using global::Noesis;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace Caliburn.Noesis
{
    /// <summary>
    /// Provides extension methods for the <see cref="DependencyObject" /> type.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>Goes up the visual tree, looking for an ancestor of the specified <see cref="Type" />.</summary>
        /// <typeparam name="T">The <see cref="Type" /> to look for.</typeparam>
        /// <param name="dependencyObject">The starting point.</param>
        /// <returns>The visual ancestor, if any, of the specified starting point and <see cref="Type" />.</returns>
        public static T FindVisualAncestor<T>(this DependencyObject dependencyObject)
            where T : DependencyObject
        {
            if (dependencyObject == null)
            {
                return null;
            }

            do
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            while ((dependencyObject != null) && !(dependencyObject is T));

            return dependencyObject as T;
        }
    }
}
