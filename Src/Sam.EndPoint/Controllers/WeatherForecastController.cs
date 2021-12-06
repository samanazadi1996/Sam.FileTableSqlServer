using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sam.Persistence;

namespace Sam.EndPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public WeatherForecastController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpPost]
        public IActionResult post(IFormFile file)
        {
            var ppp = databaseContext.Table1.CreateAsync(new FileTableFramework.Dtos.CreateFileTableDto(file));
            return Ok();
        }
    }
}
