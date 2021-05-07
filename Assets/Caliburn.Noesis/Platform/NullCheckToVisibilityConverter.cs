namespace Caliburn.Noesis
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Data;
#endif
    using System;
    using System.Globalization;

    /// <summary>Converter used to set a visibility depending on whether a value is null.</summary>
    public class NullCheckToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Hidden;
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