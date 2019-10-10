using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Timer
{
    public class TicksToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case long l: return TimeSpan.FromTicks(l).TotalSeconds.ToString("#0.0s");
                case double d: return TimeSpan.FromTicks(System.Convert.ToInt64(d)).TotalSeconds.ToString("#0.0s");
                default: return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
