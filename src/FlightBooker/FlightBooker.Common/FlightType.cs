using System.ComponentModel;

namespace FlightBooker.Common
{
    public enum FlightType : int
    {
        [Description("one-way flight")]
        OneWay = 0,

        [Description("return flight")]
        Return = 1
    }
}
