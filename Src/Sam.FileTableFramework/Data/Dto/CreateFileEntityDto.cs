using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Sam.FileTableFramework.Data.Dto
{
    public class CreateFileEntityDto
    {
        public string FileName { get; private set; }
        public Stream Stream { get; private set; }

        public CreateFileEntityDto(IFormFile file, bool keepFileName = false)
        {
            FileName = keepFileName ? file.FileName : Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf(".", StringComparison.Ordinal)..];
            Stream = file.OpenReadStream();
        }

        public CreateFileEntityDto(Stream stream, string fileName)
        {
            FileName = fileName;
            Stream = stream;
        }
    }
}
