using System.Net.Http;
using System.Text.Json;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "0bf72d54fd24457e8dc104910251005";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WeatherData>> GetLastThreeDaysWeatherAsync()
        {
            var weatherDataList = new List<WeatherData>();
            for (int i = 1; i <= 3; i++)
            {
                var date = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd");
                var response = await _httpClient.GetAsync($"http://api.weatherapi.com/v1/history.json?key={_apiKey}&q=Budapest&dt={date}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(content);
                    var day = doc.RootElement.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("day");
                    var condition = day.GetProperty("condition").GetProperty("text").GetString();
                    var avgTemp = day.GetProperty("avgtemp_c").GetDouble();
                    var maxWind = day.GetProperty("maxwind_kph").GetDouble();

                    weatherDataList.Add(new WeatherData
                    {
                        Date = date,
                        Condition = condition,
                        AvgTempC = avgTemp,
                        MaxWindKph = maxWind
                    });
                }
            }
            return weatherDataList;
        }
    }
}
