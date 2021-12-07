using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sam.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sam.EndPoint.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public FileController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<string>> GetAll()
        {
            var result = await databaseContext.Table1.GetAllAsync();

            return result.Select(p => p.name);
        }

        [HttpGet("Download/{name}")]
        public async Task<IActionResult> Download(string name)
        {
            var result = await databaseContext.Table1.FindByNameAsync(name);

            return File(result.file_stream, System.Net.Mime.MediaTypeNames.Application.Octet, result.name);
        }

        [HttpPost("Upload")]
        public async Task<string> Upload(IFormFile file)
        {
            return await databaseContext.Table1.CreateAsync(new FileTableFramework.Dtos.CreateFileTableDto(file));
        }

    }
}
