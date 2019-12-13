using FlightBooker.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FlightBooker
{
    /// <summary>
    /// <see cref="ConverterBase"/> used to convert a <see cref="FlightType"/> to/from an <see cref="int"/>
    /// </summary>
    /// <remarks>
    /// As noted [here](https://stackoverflow.com/questions/38798708/binding-combobox-to-enum-dictionary-in-uwp)
    /// and [here](https://stackoverflow.com/questions/35599479/combobox-does-not-select-binding-value-initially)
    /// the UWP combo-box has an issue where SelectedValue doesn't work with enumeration types.
    /// </remarks>
    public class FlightTypeIndexConverter : ConverterBase
    {
        private object Convert(object value)
        {
            switch(value)
            {
                case FlightType flightType: return (int)flightType;
                case int i: return (FlightType)i;
                default: return UnsetValue;
            };
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
