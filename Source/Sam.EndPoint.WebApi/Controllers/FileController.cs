using Microsoft.AspNetCore.Mvc;
using Sam.Persistence;
using System.Net.Mime;

namespace Sam.EndPoint.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController(DatabaseContext databaseContext) : ControllerBase
    {

        [HttpGet("GetPaged/{page}/{pageCount}")]
        public async Task<IActionResult> GetPaged(int page, int pageCount)
        {
            return Ok(await databaseContext.Table1.GetPagedListAsync(page, pageCount));
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await databaseContext.Table1.GetAllAsync());
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count()
        {
            return Ok(await databaseContext.Table1.CountAsync());
        }

        [HttpGet("Download/{name}")]
        public async Task<IActionResult> Download(string name)
        {
            var result = await databaseContext.Table1.FindByNameAsync(name);

            if (result is null)
                return NotFound(name);

            return File(result.file_stream!, MediaTypeNames.Application.Octet, result.name);
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf(".", StringComparison.Ordinal)..];
            var stream = file.OpenReadStream();
            return Ok(await databaseContext.Table1.CreateAsync(fileName, stream));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string name)
        {
            return Ok(await databaseContext.Table1.RemoveByNameAsync(name));
        }
    }
}
