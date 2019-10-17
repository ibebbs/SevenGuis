using System;

namespace Timer
{
    public class LongToDoubleConverter : ConverterBase
    {
        private object Convert(object value)
        {
            switch (value)
            {
                case long l: return System.Convert.ToDouble(l);
                case double d: return System.Convert.ToInt64(d);
                default: return UnsetValue;
            }
        }

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Convert(value);
        }
    }
}
