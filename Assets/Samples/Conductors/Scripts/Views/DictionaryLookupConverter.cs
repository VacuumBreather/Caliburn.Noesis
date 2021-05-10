namespace Caliburn.Noesis.Samples.Conductors.Views
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>Converts a key-value to its corresponding value in a dictionary provided as the parameter.</summary>
    /// <seealso cref= IValueConverter" />
    public class DictionaryLookupConverter : IValueConverter
    {
        #region IValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) || !(parameter is IDictionary dictionary))
            {
                return DependencyProperty.UnsetValue;
            }

            return dictionary.Contains(value) ? dictionary[value] : DependencyProperty.UnsetValue;
        }

        /// <inheritdoc />
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}