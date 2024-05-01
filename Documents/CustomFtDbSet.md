# [Building a File Management Application with ASP.NET Core and SQL Server FileTable](../README.md) - Create Custom FtDbSet

# Introduction

You may need to create your own advanced FtDbSet class, this article will teach you how to create one.

``` C#
using Sam.FileTableFramework.Context;
using Sam.FileTableFramework.Entities;
using Sam.FileTableFramework.Extentions;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sam.Persistence
{
    public class DatabaseContext : FileTableDBContext
    {
        public FtDbSet Table1 { get; set; }
        public FtDbSet Table2 { get; set; }
        public AdvancedFtDbSet Table3 { get; set; }
    }

    public class AdvancedFtDbSet : FtDbSet
    {
        public override Task<int> Count()
        {
            return base.Count();
        }

        public virtual async Task<FileEntity?> GetByStreamId(Guid streamId)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                string sqlQuery = $"SELECT TOP 1 * FROM [{TableName}] WHERE [stream_id] = '{streamId}'";

                return await connection.GetFirst<FileEntity>(sqlQuery);
            }
        }
    }
}
```

And use it like this

``` c#
using Microsoft.AspNetCore.Mvc;
using Sam.Persistence;
using System.Net.Mime;

namespace Sam.EndPoint.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController(DatabaseContext databaseContext) : ControllerBase
    {
        [HttpGet("GetByStreamId/{streamId}")]
        public async Task<IActionResult> GetByStreamId(Guid streamId)
        {
            var result = await databaseContext.Table3.GetByStreamId(streamId);

            if (result is null)
                return NotFound(streamId);

            return File(result.file_stream!, MediaTypeNames.Application.Octet, result.name);
        }
    }
}
```