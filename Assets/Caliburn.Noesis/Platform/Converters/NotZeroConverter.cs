namespace Caliburn.Noesis.Assets.Caliburn.Noesis.Platform.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NotZeroConverter : IValueConverter
    {
        #region IValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse((value ?? "").ToString(), out var val))
            {
                return Math.Abs(val) > 0.0;
            }

            return null;
        }

        /// <inheritdoc />
        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}