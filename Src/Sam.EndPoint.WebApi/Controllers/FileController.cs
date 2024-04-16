using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sam.FileTableFramework.Context;
using Sam.Persistence;
using System.IO;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sam.EndPoint.WebApi.Controllers
{
    [ApiController, Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly DatabaseContext databaseContext;

        public FileController(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

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
            return Ok(await databaseContext.Table1.Count());
        }

        [HttpGet("Download/{name}")]
        public async Task<IActionResult> Download(string name)
        {
            var result = await databaseContext.Table1.FindByNameAsync(name);

            if (result is null)
                return NotFound(name);

            return File(result.file_stream, MediaTypeNames.Application.Octet, result.name);
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await databaseContext.Table1.CreateAsync(new CreateFileEntityDto()
            {
                FileName =  Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf(".", StringComparison.Ordinal)..],
                Stream = file.OpenReadStream()
            }));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string name)
        {
            return Ok(await databaseContext.Table1.RemoveByNameAsync(name));
        }

    }
}
