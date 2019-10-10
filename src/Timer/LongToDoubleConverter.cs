using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Timer
{
    public class LongToDoubleConverter : IValueConverter
    {
        private object Convert(object value)
        {
            switch (value)
            {
                case long l: return System.Convert.ToDouble(l);
                case double d: return System.Convert.ToInt64(d);
                default: return DependencyProperty.UnsetValue;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }
    }
}
