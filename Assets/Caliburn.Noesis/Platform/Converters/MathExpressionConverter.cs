namespace Caliburn.Noesis.Converters
{
#if UNITY_5_5_OR_NEWER
    using global::Noesis;
#else
    using System.Windows;
    using System.Windows.Data;
#endif
    using System;
    using System.Globalization;
    using Jace;

    /// <summary>Converts a mathematical expression into its result.</summary>
    /// <seealso cref="IValueConverter" />
    /// <seealso cref="IMultiValueConverter" />
    public class MathExpressionConverter : IValueConverter, IMultiValueConverter
    {
        #region Constants and Fields

        private static readonly CalculationEngine CalculationEngine =
            new CalculationEngine(CultureInfo.InvariantCulture);

        #endregion

        #region IMultiValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object[] values,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (!(parameter is string expression) || string.IsNullOrEmpty(expression) || values is null)
            {
                return DependencyProperty.UnsetValue;
            }

            try
            {
                expression = string.Format(CultureInfo.InvariantCulture, expression, values);

                return CalculationEngine.Calculate(expression);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        /// <inheritdoc />
        public object[] ConvertBack(object value,
                                    Type[] targetTypes,
                                    object parameter,
                                    CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IValueConverter Implementation

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string expression) || string.IsNullOrEmpty(expression))
            {
                return DependencyProperty.UnsetValue;
            }

            try
            {
                expression = string.Format(CultureInfo.InvariantCulture, expression, value);

                return CalculationEngine.Calculate(expression);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
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