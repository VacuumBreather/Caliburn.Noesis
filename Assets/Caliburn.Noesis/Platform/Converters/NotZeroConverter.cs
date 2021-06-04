namespace Caliburn.Noesis.Assets.Caliburn.Noesis.Platform.Converters
{
    using System;
    using System.Globalization;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows.Data;
#endif

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