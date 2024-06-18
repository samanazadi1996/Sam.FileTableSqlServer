using Microsoft.AspNetCore.Mvc;
using Sam.EndPoint.WebApi.Models;
using Sam.FileTableFramework.Linq;
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
            var query = databaseContext.Table1;

            var result = await query
                .Skip(page)
                .Take(pageCount)
                .OrderBy(p => p.name)
                .Select(p => new FileEntityDto()
                {
                    Name = p.name,
                    Size = p.cached_file_size,
                    Id = p.stream_id,
                    Type = p.file_type
                })
                .ToListAsync<FileEntityDto>();

            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await databaseContext.Table1.ToListAsync();

            return Ok(result);
        }

        [HttpGet("Count")]
        public async Task<IActionResult> Count()
        {
            return Ok(await databaseContext.Table1.CountAsync());
        }

        [HttpGet("Download/{name}")]
        public async Task<IActionResult> Download(string name)
        {
            var result = await databaseContext.Table1.Where($"name = '{name}'").ToListAsync();

            if (result is null || !result.Any())
                return NotFound(name);

            var entity = result.First();

            return File(entity.file_stream!, MediaTypeNames.Application.Octet, entity.name);
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileName = Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf(".", StringComparison.Ordinal)..];
            var stream = file.OpenReadStream();

            databaseContext.Table1.Create(fileName, stream);
            await databaseContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string name)
        {
            var result = await databaseContext.Table1.Where($"name = '{name}'").ToListAsync();

            if (result is null || !result.Any())
                return NotFound(name);

            databaseContext.Table1.Remove(result.First());
            await databaseContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("TestQueryString")]
        public async Task<IActionResult> TestQueryString()
        {

            var query = databaseContext.Table1;

            var result = query
                .Take(3)
                .Skip(2)
                .Where("name = 'saman'")
                .OrderBy(p => p.name).OrderBy(p => p.is_archive).OrderByDescending(p => p.stream_id)
                .Select(p => new FileEntityDto()
                {
                    Name = p.name,
                    Size = p.cached_file_size,
                    Id = p.stream_id,
                    Type = p.file_type
                });

            return Ok(new
            {
                Query = result.ToQueryString(),
                Data = await result.ToListAsync<FileEntityDto>()
            });
        }

    }
}
