using Microsoft.AspNetCore.Mvc;
using WeatherForecastAPI.Services;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherService _weatherService;
        public WeatherForecastController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<ActionResult<WeatherData>> GetForecast()
        {
            var pastWeather = await _weatherService.GetLastThreeDaysWeatherAsync();

            // Egyszerű valószínűségi modell: a leggyakoribb időjárási típus alapján előrejelzés
            var mostCommonCondition = pastWeather
                .GroupBy(w => w.Condition)
                .OrderByDescending(g => g.Count())
                .First().Key;

            var avgTemp = pastWeather.Average(w => w.AvgTempC);
            var avgWind = pastWeather.Average(w => w.MaxWindKph);

            var forecast = new WeatherData
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Condition = mostCommonCondition,
                AvgTempC = avgTemp,
                MaxWindKph = avgWind
            };

            return Ok(forecast);
        }
    }
}
