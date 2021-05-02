#if NOESIS
namespace Caliburn.Noesis.Platform
{
    #region Using Directives

    using System;
    using System.Globalization;

    #endregion

    /// <summary>
    ///     Converter used to set a visibility depending on whether a value is null.
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        #region Public Methods

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Hidden;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
#endif