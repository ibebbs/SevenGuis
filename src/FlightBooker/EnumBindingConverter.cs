using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace FlightBooker
{
    public class EnumBindingConverter : IValueConverter
    {
        public static string Description(Enum e)
        {
            return e.GetType()
                .GetField(e.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .OfType<DescriptionAttribute>()
                .Select(attribute => attribute.Description)
                .FirstOrDefault() ?? e.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Enum
                .GetValues(value.GetType())
                .Cast<Enum>()
                .Select(e => new KeyValuePair<string, Enum>(Description(e), e))
                .ToArray();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
