using System;

namespace TemperatureConverter
{
    public class StringToIntConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case string text: return Int32.TryParse(text, out int number) ? number : UnsetValue;
                default: return UnsetValue;
            }
        }
    }
}
