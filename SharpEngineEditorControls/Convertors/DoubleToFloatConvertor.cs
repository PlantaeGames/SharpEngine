using System;
using System.Windows.Data;
using System.Globalization;

namespace SharpEngineEditorControls.Convertors
{
    public sealed class DoubleToFloatConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = System.Convert.ToDouble(value);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = System.Convert.ToSingle((double)value);
            return result;
        }
    }
}