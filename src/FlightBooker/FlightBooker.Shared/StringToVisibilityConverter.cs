using System;

namespace FlightBooker.Shared
{
    public class StringToVisibilityConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case string text: return string.IsNullOrWhiteSpace(text) ? Collapsed : Visible;
                default: return UnsetValue;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
