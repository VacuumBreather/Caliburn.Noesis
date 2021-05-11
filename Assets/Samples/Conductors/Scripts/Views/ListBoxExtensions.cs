namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>Provides attached properties used to display header content in a <see cref="ListBox" />.</summary>
    public static class ListBoxExtensions
    {
        #region Constants and Fields

        /// <summary>The header property.</summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(HeaderProperty)),
                typeof(object),
                typeof(ListBoxExtensions),
                new PropertyMetadata(default(object)));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.RegisterAttached(
                PropertyNameHelper.GetName(nameof(HeaderTemplateProperty)),
                typeof(DataTemplate),
                typeof(ListBoxExtensions),
                new PropertyMetadata(default(DataTemplate)));

        #endregion

        #region Public Methods

        /// <summary>Gets the header content of the specified <see cref="ListBox" />.</summary>
        /// <param name="listBox">The listBox.</param>
        /// <returns>The header content.</returns>
        public static object GetHeader(ListBox listBox)
        {
            return listBox.GetValue(HeaderProperty);
        }

        /// <summary>Gets the template for the header content of the specified <see cref="ListBox" />.</summary>
        /// <param name="listBox">The list box.</param>
        /// <returns>The template for the header content.</returns>
        public static DataTemplate GetHeaderTemplate(ListBox listBox)
        {
            return (DataTemplate)listBox.GetValue(HeaderTemplateProperty);
        }

        /// <summary>Sets the header content for the specified <see cref="ListBox" />.</summary>
        /// <param name="listBox">The listBox.</param>
        /// <param name="value">The header content.</param>
        public static void SetHeader(ListBox listBox, object value)
        {
            listBox.SetValue(HeaderProperty, value);
        }

        /// <summary>Sets the template for the header content of the specified <see cref="ListBox" />.</summary>
        /// <param name="listBox">The list box.</param>
        /// <param name="value">The template for the header content.</param>
        public static void SetHeaderTemplate(ListBox listBox, DataTemplate value)
        {
            listBox.SetValue(HeaderTemplateProperty, value);
        }

        #endregion
    }
}