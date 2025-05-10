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

            // Mai időjárás
            var todayWeather = pastWeather.First();

            // Holnapi előrejelzés
            string nextCondition = PredictNextCondition(todayWeather.Condition);

            var forecast = new WeatherData
            {
                Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                Condition = nextCondition,
                AvgTempC = todayWeather.AvgTempC,
                MaxWindKph = todayWeather.MaxWindKph,
                Icon = GetWeatherIcon(nextCondition)
            };

            return Ok(new { today = todayWeather, forecast = forecast });
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
                "Blizzard" => "Heavy snow",
                "Heavy snow" => "Patchy snow",
                "Patchy snow" => "Light snow",
                "Light snow" => "Rain",
                "Rain" => "Patchy rain",
                "Patchy rain" => "Light rain",
                "Light rain" => "Partly cloudy",
                "Partly cloudy" => "Sunny",
                _ => "Sunny"
            };
        }

        private string WorsenWeather(string condition)
        {
            return condition switch
            {
                "Sunny" => "Partly cloudy",
                "Partly cloudy" => "Light rain",
                "Light rain" => "Patchy rain",
                "Patchy rain" => "Rain",
                "Rain" => "Light snow",
                "Light snow" => "Patchy snow",
                "Patchy snow" => "Heavy snow",
                "Heavy snow" => "Blizzard",
                _ => "Blizzard"
            };

        }

        private string GetWeatherIcon(string condition)
        {
            return condition switch
            {
                "Sunny" => "https://cdn.weatherapi.com/weather/64x64/day/113.png",
                "Partly cloudy" => "https://cdn.weatherapi.com/weather/64x64/day/116.png",
                "Patchy rain" => "https://cdn.weatherapi.com/weather/64x64/day/176.png",
                "Light rain" => "https://cdn.weatherapi.com/weather/64x64/day/296.png",
                "Rain" => "https://cdn.weatherapi.com/weather/64x64/day/308.png",
                "Light snow" => "https://cdn.weatherapi.com/weather/64x64/day/326.png",
                "Patchy snow" => "https://cdn.weatherapi.com/weather/64x64/day/179.png",
                "Heavy snow" => "https://cdn.weatherapi.com/weather/64x64/day/338.png",
                "Blizzard" => "https://cdn.weatherapi.com/weather/64x64/day/371.png",
                _ => "https://cdn.weatherapi.com/weather/64x64/day/122.png"
            };
        }
    }
}
