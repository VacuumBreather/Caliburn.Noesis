namespace Caliburn.Noesis
{
    using System;
    using System.Globalization;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;

#else
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>Coverts a null value to <see cref="Visibility.Collapsed" />.</summary>
    public class NullToCollapsedConverter : IValueConverter
    {
        #region IValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
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