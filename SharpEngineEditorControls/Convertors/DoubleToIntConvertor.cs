using System;
using System.Windows.Data;
using System.Globalization;

namespace SharpEngineEditorControls.Convertors
{
    public sealed class DoubleToIntConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = Math.Clamp((int)value, int.MinValue, int.MaxValue);
            var result = (double)target;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = Math.Clamp((double)value, double.MinValue, double.MaxValue);
            var result = (int)target;
            return result;
        }
    }
}