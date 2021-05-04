namespace Caliburn.Noesis
{
    #region Using Directives

    using System;
    using System.Globalization;
    using global::Noesis;

    #endregion

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