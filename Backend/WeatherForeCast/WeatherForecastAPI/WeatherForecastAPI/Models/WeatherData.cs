namespace WeatherForecastAPI.Models
{
    public class WeatherData
    {
        public string Date { get; set; }
        public string Condition { get; set; }
        public double AvgTempC { get; set; }
        public double MaxWindKph { get; set; }
        public string Icon { get; set; }
        public string Humidity { get; set; }
        public string WindDirection { get; set; }
        public string FrontInfo { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public string Moonrise { get; set; }
        public string Moonset { get; set; }
    }
}
