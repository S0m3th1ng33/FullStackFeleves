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
                try
                {
                    var date = DateTime.UtcNow.AddDays(-i).ToString("yyyy-MM-dd");
                    var response = await _httpClient.GetAsync($"http://api.weatherapi.com/v1/history.json?key={_apiKey}&q=Budapest&dt={date}&aqi=yes");
                    var current_response = await _httpClient.GetAsync($"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q=Budapest&dt={date}&aqi=yes");
                    Console.WriteLine($"Válasz tartalom ({date}): {response}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        using JsonDocument doc_Forecast = JsonDocument.Parse(content);

                        var current = await current_response.Content.ReadAsStringAsync();
                        using JsonDocument doc_Current = JsonDocument.Parse(current);

                        var day = doc_Forecast.RootElement.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("day");
                        var condition = day.GetProperty("condition").GetProperty("text").GetString();
                        var avgTemp = day.GetProperty("avgtemp_c").GetDouble();
                        var maxWind = day.GetProperty("maxwind_kph").GetDouble();
                        var icon = day.GetProperty("condition").GetProperty("icon").GetString();
                        var astro = doc_Forecast.RootElement.GetProperty("forecast").GetProperty("forecastday")[0].GetProperty("astro");
                        var moonPhase = astro.GetProperty("moon_phase").GetString() ?? "Unknown";

                        var airQuality = doc_Current.RootElement.GetProperty("current").GetProperty("air_quality");
                        var co = airQuality.GetProperty("co").GetSingle();
                        var ozone = airQuality.GetProperty("o3").GetSingle();
                        var no2 = airQuality.GetProperty("no2").GetSingle();
                        var epa = airQuality.GetProperty("us-epa-index").GetInt32();

                        if (icon.StartsWith("//"))
                            icon = "https:" + icon;

                        weatherDataList.Add(new WeatherData
                        {
                            Date = date,
                            Condition = condition,
                            AvgTempC = avgTemp,
                            MaxWindKph = maxWind,
                            Icon = icon,
                            Sunrise = astro.GetProperty("sunrise").GetString(),
                            Sunset = astro.GetProperty("sunset").GetString(),
                            Moonrise = astro.GetProperty("moonrise").GetString(),
                            Moonset = astro.GetProperty("moonset").GetString(),
                            MoonPhase = moonPhase,
                            CO = co,
                            Ozone = ozone,
                            NO2 = no2,
                            EpaIndex = epa,
                            FrontInfo = avgTemp > 20  && maxWind < 14 ? "Melegfront esélyes" : "Hidegfront esélyes"
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch data for {date}. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception while fetching weather data: {e.Message}");
                }

                if (weatherDataList.Count == 0)
                {
                    Console.WriteLine("Warning: No weather data was retrieved.");
                }

            }
            return weatherDataList;
        }
    }
}
