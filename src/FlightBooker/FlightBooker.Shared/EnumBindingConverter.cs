using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FlightBooker
{
    public class EnumBindingConverter : ConverterBase
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

        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            return Enum
                .GetValues(value.GetType())
                .Cast<Enum>()
                .Select(e => new KeyValuePair<string, Enum>(Description(e), e))
                .ToArray();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
