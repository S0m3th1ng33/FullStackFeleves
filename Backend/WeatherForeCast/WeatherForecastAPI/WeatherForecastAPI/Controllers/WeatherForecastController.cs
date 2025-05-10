using Microsoft.AspNetCore.Mvc;
using WeatherForecastAPI.Services;
using WeatherForecastAPI.Models;
using System;

namespace WeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        private readonly Random random;
        public WeatherForecastController(WeatherService weatherService)
        {
            _weatherService = weatherService;
            random = new Random();
        }

        [HttpGet]
        public async Task<ActionResult<WeatherData>> GetForecast()
        {
            var pastWeather = await _weatherService.GetLastThreeDaysWeatherAsync();

            if (pastWeather.Select(w => w.Condition).Distinct().Count() == 1)
            {
                string currentCondition = pastWeather[0].Condition;
                string nextCondition = PredictNextCondition(currentCondition);

                var avgTemp = pastWeather.Average(w => w.AvgTempC);
                var avgWind = pastWeather.Average(w => w.MaxWindKph);

                var forecast = new WeatherData
                {
                    Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    Condition = nextCondition,
                    AvgTempC = avgTemp,
                    MaxWindKph = avgWind,
                    Icon = GetWeatherIcon(nextCondition)
                };

                return Ok(forecast);
            }

            var randomWeather = new[] { "Sunny","Partly cloudy", "Overcast", "Rain", "Snow" }[random.Next(4)];
            return Ok(new WeatherData
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Condition = randomWeather,
                AvgTempC = pastWeather.Average(w => w.AvgTempC),
                MaxWindKph = pastWeather.Average(w => w.MaxWindKph)
            });
        }

        private string PredictNextCondition(string currentCondition)
        {
            int chance = random.Next(100);

            // 70% eséllyel marad azonos
            if (chance < 70)
                return currentCondition;

            // 20% eséllyel javul
            if (chance < 90)
                return ImproveWeather(currentCondition);

            // 10% eséllyel romlik
            return WorsenWeather(currentCondition);
        }

        private string ImproveWeather(string condition)
        {
            return condition switch
            {
                "Snowy" => "Rainy",
                "Rainy" => "Overcast",
                "Overcast" => "Partly cloudy",
                "Partly cloudy" => "Sunny",
                _ => condition // Ha már napos, nem javul tovább
            };
        }

        private string WorsenWeather(string condition)
        {
            return condition switch
            {
                "Sunny" => "Partly cloudy",
                "Partly cloudy" => "Overcast",
                "Overcast" => "Rainy",
                "Rainy" => "Snowy",
                _ => condition // Ha már esős, nem romlik tovább
            };
        }

        private string GetWeatherIcon(string condition)
        {
            return condition switch
            {
                "Sunny" => "https://cdn.weatherapi.com/weather/64x64/day/113.png",
                "Overcast" => "https://cdn.weatherapi.com/weather/64x64/day/116.png",
                "Rainy" => "https://cdn.weatherapi.com/weather/64x64/day/296.png",
                "Snowy" => "https://cdn.weatherapi.com/weather/64x64/day/338.png",
                _ => "https://cdn.weatherapi.com/weather/64x64/day/122.png"
            };
        }
    }
}
