using System;
#if __WPF__
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

namespace FlightBooker
{
    public class BooleanToBrushConverter : ConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (value)
            {
                case bool boolean: return boolean ? TrueColor : FalseColor;
                default: return UnsetValue;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        public Brush TrueColor { get; set; }

        public Brush FalseColor { get; set; }
    }
}
