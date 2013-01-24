using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace triggr
{
    /// <summary>
    /// Convert a boolean value to a Visibility (Visible/Collapsed)
    /// </summary>
    [ValueConversionAttribute(typeof(bool), typeof(Visibility))]
    public class InvertedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool v = (bool)value;
            return (v) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            return (v == Visibility.Collapsed);
        }
    }
}
