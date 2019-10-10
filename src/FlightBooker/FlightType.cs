using System.ComponentModel;

namespace FlightBooker
{
    public enum FlightType
    {
        [Description("one-way flight")]
        OneWay,

        [Description("return flight")]
        Return
    }
}
