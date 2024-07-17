using System;
using System.Windows.Data;
using System.Globalization;

namespace SharpEngineEditorControls.Convertors
{
    public sealed class DoubleToIntConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = System.Convert.ToDouble(value);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = (double)value;
            var result = 0;
            if (target <= int.MinValue)
            {
                result = int.MinValue;
            }
            else if(target >= int.MaxValue)
            {
                result = int.MaxValue;
            }
            else
                result = System.Convert.ToInt32(target);
            return result;
        }
    }
}