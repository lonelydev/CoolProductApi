using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolProductApi.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [MapToApiVersion("1")]
        public IEnumerable<WeatherForecastV1> GetV1()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV1
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [MapToApiVersion("2")]
        public IEnumerable<WeatherForecastV2> GetV2()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecastV2
            {
                Date = DateTime.Now.AddDays(index),
                Celsius= rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
