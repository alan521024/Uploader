using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace DoubleXUI.Converters
{
    public class IconButtonVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            //var flag = value == null;
            //if (value is string)
            //{
            //    flag = string.IsNullOrEmpty((string)value);
            //}
            //var inverse = (parameter as string) == "inverse";

            //if (inverse)
            //{
            //    return (flag ? Visibility.Collapsed : Visibility.Visible);
            //}
            //else
            //{
            //    return (flag ? Visibility.Visible : Visibility.Collapsed);
            //}
            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
