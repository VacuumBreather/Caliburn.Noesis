using System;
using System.Globalization;
using System.Reflection;

#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Caliburn.Noesis
{
    public class EnumFlagSetToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Enum enumFlags || value.GetType().GetCustomAttribute(typeof(FlagsAttribute)) is null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (parameter is not Enum parameterFlag ||
                parameter.GetType().GetCustomAttribute(typeof(FlagsAttribute)) is null ||
                (parameter.GetType() != value.GetType()))
            {
                return DependencyProperty.UnsetValue;
            }

            return enumFlags.HasFlag(parameterFlag) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
