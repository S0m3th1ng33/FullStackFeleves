using System.Text.Json.Serialization;

namespace WeatherForecastAPI.Models
{
    public class WeatherData
    {
        public string Date { get; set; }
        public string Condition { get; set; }
        public double AvgTempC { get; set; }
        public double MaxWindKph { get; set; }
        public string Icon { get; set; }
        public string FrontInfo { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Moonrise { get; set; }
        public string Moonset { get; set; }
        public string MoonPhase { get; set; }

        [JsonPropertyName("co")]
        public float CO { get; set; }

        [JsonPropertyName("ozone")]
        public float Ozone { get; set; }

        [JsonPropertyName("no2")]
        public float NO2 { get; set; }

        [JsonPropertyName("epaIndex")]
        public int EpaIndex { get; set; }

    }
}
