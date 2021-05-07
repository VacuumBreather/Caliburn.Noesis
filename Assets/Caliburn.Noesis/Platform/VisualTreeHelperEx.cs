namespace Caliburn.Noesis
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Media;

#endif

    /// <summary>Provides extended helper methods beyond what <see cref="VisualTreeHelper" /> provides.</summary>
    public static class VisualTreeHelperEx
    {
        #region Public Methods

        /// <summary>Gets the first visual child of type <see cref="T" />.</summary>
        /// <param name="parent">The parent to search.</param>
        /// <typeparam name="T">The type of the child element to look for.</typeparam>
        /// <returns>The first visual child of type <see cref="T" />.</returns>
        public static T GetVisualChild<T>(this DependencyObject parent)
            where T : Visual
        {
            var child = default(T);
            var numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < numVisuals; i++)
            {
                var visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T ?? GetVisualChild<T>(visual);

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        #endregion
    }
}