using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sam.FileTableFramework.Data.Dto;
using Sam.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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

        [HttpGet("GetPaged/{page}/{pageCount}")]
        public async Task<PagedListFileEntityDto> GetPaged(int page, int pageCount)
        {
            var result = await databaseContext.Table1.GetPagedListAsync(page, pageCount);

            return result;
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

            return File(result.file_stream, MediaTypeNames.Application.Octet, result.name);
        }

        [HttpPost("Upload")]
        public async Task<string> Upload(IFormFile file)
        {
            return await databaseContext.Table1.CreateAsync(new CreateFileEntityDto(file));
        }

    }
}
