using System;

namespace Timer
{
    public class TicksToSecondsConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case long l: return TimeSpan.FromTicks(l).TotalSeconds.ToString("#0.0s");
                case double d: return TimeSpan.FromTicks(System.Convert.ToInt64(d)).TotalSeconds.ToString("#0.0s");
                default: return UnsetValue;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
