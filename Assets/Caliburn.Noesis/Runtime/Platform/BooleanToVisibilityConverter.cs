﻿using System.Globalization;

namespace Caliburn.Noesis
{
    using System;
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    /// <summary>
    /// An <see cref="IValueConverter"/> which converts <see cref="bool"/> to <see cref="Visibility"/>.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value <see cref="Visibility"/> value to a boolean value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="language">The language to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            return ((Visibility)value) == Visibility.Visible;
        }
    }
}

