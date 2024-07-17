using System;
using System.Windows.Data;
using System.Globalization;

namespace SharpEngineEditorControls.Convertors
{
    public sealed class DoubleToFloatConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = Math.Clamp((float)value, float.MinValue, float.MaxValue);
            var result = (double)target;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = Math.Clamp((double)value, double.MinValue, double.MaxValue);
            var result = (float)target;
            return result;
        }
    }
}