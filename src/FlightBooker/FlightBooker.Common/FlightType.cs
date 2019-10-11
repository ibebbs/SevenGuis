using System.ComponentModel;

namespace FlightBooker.Common
{
    public enum FlightType
    {
        [Description("one-way flight")]
        OneWay,

        [Description("return flight")]
        Return
    }
}
