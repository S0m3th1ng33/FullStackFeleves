namespace WeatherForecastAPI.Models
{
    public class WeatherData
    {
        public string Date { get; set; }
        public string Condition { get; set; }
        public double AvgTempC { get; set; }
        public double MaxWindKph { get; set; }

        public string Icon { get; set; }
    }
}
