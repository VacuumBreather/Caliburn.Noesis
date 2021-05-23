namespace Caliburn.Noesis.Extensions
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>Provides extension methods for the <see cref="FrameworkElement" /> type.</summary>
    public static class FrameworkElementExtensions
    {
        #region Public Methods

        /// <summary>Gets or creates a valid namescope for this root element.</summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>A valid namescope for this root element.</returns>
        public static FrameworkElement GetNameScopeRoot(this FrameworkElement rootElement)
        {
            // Only set the namescope if the child does not already have a template XAML namescope set.
            if ((VisualTreeHelper.GetChildrenCount(rootElement) > 0) &&
                VisualTreeHelper.GetChild(rootElement, 0) is FrameworkElement frameworkElement &&
                (NameScope.GetNameScope(frameworkElement) != null))
            {
                return frameworkElement;
            }

            if (NameScope.GetNameScope(rootElement) is null)
            {
                NameScope.SetNameScope(rootElement, new NameScope());
            }

            return rootElement;
        }

        #endregion
    }
}