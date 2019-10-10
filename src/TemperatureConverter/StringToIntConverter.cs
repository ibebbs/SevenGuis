using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TemperatureConverter
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case string text: return Int32.TryParse(text, out int number) ? number : DependencyProperty.UnsetValue;
                default: return DependencyProperty.UnsetValue;
            }
        }
    }
}
