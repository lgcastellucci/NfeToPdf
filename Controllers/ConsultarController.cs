using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NfeToPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsultarController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ConsultarController> _logger;

        public ConsultarController(ILogger<ConsultarController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Consultar> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Consultar
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
