# Building a File Management Application with ASP.NET Core and SQL Server FileTable 

[![Badge CI](https://github.com/samanazadi1996/Sam.FileTableSqlServer/workflows/.NET/badge.svg)](https://github.com/samanazadi1996/Sam.FileTableSqlServer/actions)
[![Badge NuGet](https://img.shields.io/nuget/vpre/Sam.FileTableFramework.svg)](https://www.nuget.org/packages/Sam.FileTableFramework)

# Introduction

In today's digital world, effective file management is crucial for individuals and organizations alike. Developing a file management application can help streamline file organization and improve accessibility. In this article, we will explore the intricacies and implementation steps of building a file management application using ASP.NET Core and SQL Server FileTable.

# Getting Started

1. You'll want to enable SQL Server FILESTREAM. Follow the instructions in [this link](./Documents/EnableSqlServerFILESTREAM.md) for the necessary guidance on activation.

2. To install the Sam.File Table Framework package, simply use the following command
   - .NET CLI

        ``` sh
        dotnet add package Sam.FileTableFramework --version 2.1.0
        ```
   - Package Manager

        ``` sh
        NuGet\Install-Package Sam.FileTableFramework -Version 2.1.0
        ```

   - Package Reference
        ``` xml
        <PackageReference Include="Sam.FileTableFramework" Version="2.1.0" />
        ```
    
    - Paket CLI
        ``` sh
        paket add Sam.FileTableFramework --version 2.1.0
        ```

3. Create your DbContext by inheriting from FileTableDbContext. Then, define a FtDbSet property for your tables.
    ``` c#
    using Sam.FileTableFramework.Context;

    namespace Sam.Persistence
    {
        public class DatabaseContext: FileTableDBContext
        {
            public FtDbSet Table1 { get; set; }
            public FtDbSet Table2 { get; set; }
            public FtDbSet Table3 { get; set; }
        }
    }
    ```
    You can have your advanced FtDbSet, for this, refer to [this link](./Documents/CustomFtDbSet.md)

4. Register your DatabaseContext class in Program.cs
    ``` c#
    // ...
    builder.Services.AddFileTableDBContext<DatabaseContext>(o =>
        o.ConnectionString = "Data Source =.; Initial Catalog = DatabaseName; Integrated Security = true");
    // ...

5. You can create the database by adding the following code when the project is running
    ``` c#
    // ...
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        services.GetRequiredService<DatabaseContext>().Migrate();
    }
    // ...
    ```
6. Now you can inject DatabaseContext in your classes and use your tables, for example, see the source code below
    ``` c#
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
                var skip = (page - 1) * pageCount;

                var query = databaseContext.Table1;

                var result = await query
                    .Skip(skip)
                    .Take(pageCount)
                    .OrderBy(p => p.name)
                    .ToListAsync(p => new FileEntityDto()
                    {
                        Name = p.name,
                        Size = p.cached_file_size,
                        Id = p.stream_id,
                        Type = p.file_type
                    });

                return Ok(result);
            }

            [HttpGet("GetAll")]
            public async Task<IActionResult> GetAll()
            {
                var query = databaseContext.Table1;

                var result = await query
                    .ToListAsync(p => new FileEntityDto()
                    {
                        Name = p.name,
                        Size = p.cached_file_size,
                        Id = p.stream_id,
                        Type = p.file_type
                    });

                return Ok(result);
            }

            [HttpGet("Count")]
            public async Task<IActionResult> Count()
            {
                var query = databaseContext.Table1;
                return Ok(await query.CountAsync());
            }

            [HttpGet("Download/{name}")]
            public async Task<IActionResult> Download(string name)
            {
                var entity = await databaseContext.Table1.Where($"name = '{name}'").FirstOrDefaultAsync();

                if (entity is null)
                    return NotFound(nameof(NotFound));

                return File(entity.file_stream!, MediaTypeNames.Application.Octet, entity.name);
            }

            [HttpPost("Upload")]
            public async Task<IActionResult> Upload(IFormFile file)
            {
                var fileName = Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf('.')..];
                var stream = file.OpenReadStream();

                await databaseContext.Table1.CreateAsync(fileName, stream);

                return Ok(fileName);
            }

            [HttpDelete("Delete")]
            public async Task<IActionResult> Delete(string name)
            {
                var entity = await databaseContext.Table1.Where($"name = '{name}'").FirstOrDefaultAsync();

                if (entity is null)
                    return NotFound(nameof(NotFound));

                var temp = await databaseContext.Table1.RemoveAsync(entity);

                return Ok(temp);
            }

            [HttpGet("TestQueryString")]
            public async Task<IActionResult> TestQueryString()
            {

                var query = databaseContext.Table1;

                var result = query
                    .Take(3)
                    .Skip(2)
                    .Where("name = 'TestName'")
                    .OrderBy(p => p.name)
                    .OrderBy(p => p.is_archive)
                    .OrderByDescending(p => p.stream_id)
                    .OrderBy(p => p.creation_time)
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
                    Data = await result.ToListAsync(p => new FileEntityDto()
                    {
                        Name = p.name,
                        Size = p.cached_file_size,
                        Id = p.stream_id,
                        Type = p.file_type
                    })
                });
            }

        }

        public class FileEntityDto
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Type { get; set; }
            public long Size { get; set; }
        }
    }
    ```

# Conclusion

In this article, we delved into creating a file management application using ASP.NET Core and SQL Server FileTable. This application provides functionalities for organizing and managing files in a web environment. Leveraging modern technologies and tools like FileTable, we were able to build a secure, reliable, and high-performance application.


# Support
If you are having problems, please let me know by [raising a new issue](https://github.com/samanazadi1996/Sam.FileTableSqlServer/issues).

# License
This project is licensed with the [MIT license](https://github.com/samanazadi1996/Sam.FileTableSqlServer?tab=MIT-1-ov-file#readme).
