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

            if (pastWeather == null || pastWeather.Count == 0)
                return StatusCode(500, "Nem érkeztek adatok a WeatherService-ből.");

            // Mai időjárás
            var todayWeather = pastWeather.First();

            WeatherData forecast = ForecastWeather(pastWeather);

            return Ok(new { today = todayWeather, forecast = forecast });
        }

        private WeatherData ForecastWeather(List<WeatherData> pastWeather)
        {
            string nextCondition;
            double nextTemp;
            double nextWind;
            
            bool isConsistent = pastWeather.GroupBy(w => w.Condition).Any(g => g.Count() == 3);
            int chance = random.Next(100);

            if (isConsistent)
            {
                // Azonos időjárás esetén
                string consistentCondition = pastWeather.GroupBy(w => w.Condition).OrderByDescending(g => g.Count()).First().Key;
                double avgTemp = pastWeather.Average(w => w.AvgTempC);
                double avgWind = pastWeather.Average(w => w.MaxWindKph);

                if (chance < 70)  // 70% eséllyel marad azonos
                {
                    nextCondition = consistentCondition;
                    nextTemp = avgTemp;
                    nextWind = avgWind;
                }
                else if (chance < 90)  // 20% eséllyel javul
                {
                    nextCondition = ImproveWeather(consistentCondition);
                    nextTemp = avgTemp + random.NextDouble() * 2 + 1;
                    nextWind = Math.Max(0, avgWind - (random.NextDouble() * 2 + 1));
                }
                else  // 10% eséllyel romlik
                {
                    nextCondition = WorsenWeather(consistentCondition);
                    nextTemp = avgTemp - (random.NextDouble() * 2 + 1);
                    nextWind = avgWind + random.NextDouble() * 2 + 1;
                }
            }
            else
            {
                // Változatos időjárás esetén véletlenszerű
                var possibleConditions = new List<string> { "Sunny", "Partly cloudy", "Light rain", "Rain", "Light snow", "Heavy snow", "Blizzard" };
                nextCondition = possibleConditions[random.Next(possibleConditions.Count)];

                nextTemp = pastWeather.Average(w => w.AvgTempC) + random.NextDouble() * 2 - 1;
                nextWind = pastWeather.Average(w => w.MaxWindKph) + random.NextDouble() * 2 - 1;
            }

            // Holnapi előrejelzés adatai
            WeatherData nextDay = new WeatherData
            {
                Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                Condition = nextCondition,
                AvgTempC = Math.Round(nextTemp, 1),  // Lekerekítés 1 tizedesjegyre
                MaxWindKph = Math.Round(nextWind, 1),  // Lekerekítés 1 tizedesjegyre
                Icon = GetWeatherIcon(nextCondition)
            };

            return nextDay;

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
