using System;
using System.Collections.Generic;
using System.Globalization;

#if UNITY_5_5_OR_NEWER
using global::Noesis;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace Caliburn.Noesis
{
    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AreEqual(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }

        private static bool AreEqual<T>(T first, T second)
        {
            return EqualityComparer<T>.Default.Equals(first, second);
        }
    }
}
