using System;

namespace TemperatureConverter.Common
{
    public static class Domain
    {
        public static int ConvertCelciusToFahrenheit(int celcius)
        {
            return Convert.ToInt32(celcius * (9.0 / 5.0) + 32);
        }

        public static int ConvertFahrenheitToCelcius(int fahrenheit)
        {
            return Convert.ToInt32((fahrenheit - 32) * (5.0 / 9.0));
        }
    }
}
