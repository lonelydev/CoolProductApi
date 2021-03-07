using System;

namespace CoolProductApi
{
    public class WeatherForecastV2
    {
        public DateTime Date { get; set; }

        public int Celsius { get; set; }

        public int Farenheit => 32 + (int)(Celsius / 0.5556);

        public string Summary { get; set; }
    }
}