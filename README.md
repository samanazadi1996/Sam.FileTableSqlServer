# Building a File Management Application with ASP.NET Core and SQL Server FileTable 

# Introduction

In today's digital world, effective file management is crucial for individuals and organizations alike. Developing a file management application can help streamline file organization and improve accessibility. In this article, we will explore the intricacies and implementation steps of building a file management application using ASP.NET Core and SQL Server FileTable.

# Getting Started

1. You'll want to enable SQL Server FILESTREAM. Follow the instructions in [this link](./Documents/EnableSqlServerFILESTREAM.md) for the necessary guidance on activation.

2. To install the Sam.File Table Framework package, simply use the following command
   - .NET CLI

        ``` sh
        dotnet add package Sam.FileTableFramework -version 1.0.1
        ```
   - Package Manager

        ``` sh
        NuGet\Install-Package Sam.FileTableFramework -Version 1.0.1
        ```

   - Package Reference
        ``` c#
        <PackageReference Include="Sam.FileTableFramework" Version="1.0.1" />
        ```
    
    - Paket CLI
        ``` sh
        paket add Sam.FileTableFramework --version 1.0.1
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

4. Register your DatabaseContext class in Startup
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
                return Ok(await databaseContext.Table1.Count());
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
    ```
# Conclusion

In this article, we delved into creating a file management application using ASP.NET Core and SQL Server FileTable. This application provides functionalities for organizing and managing files in a web environment. Leveraging modern technologies and tools like FileTable, we were able to build a secure, reliable, and high-performance application.

