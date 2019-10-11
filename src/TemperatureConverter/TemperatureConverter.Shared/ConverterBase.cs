using System;
#if __WPF__
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace TemperatureConverter
{
    public abstract class ConverterBase : IValueConverter
    {
        protected static readonly object UnsetValue = DependencyProperty.UnsetValue;
#if __WPF__
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture.ThreeLetterWindowsLanguageName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture.ThreeLetterWindowsLanguageName);
        }
#endif
        public abstract object Convert(object value, Type targetType, object parameter, string language);

        public abstract object ConvertBack(object value, Type targetType, object parameter, string language);
    }
}
