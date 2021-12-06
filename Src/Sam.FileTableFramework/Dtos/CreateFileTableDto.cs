using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Sam.FileTableFramework.Dtos
{
    public class CreateFileTableDto
    {
        public string FileName { get; private set; }
        public Stream Stream { get; private set; }

        public CreateFileTableDto(IFormFile file, bool keepFileName = false)
        {
            FileName = keepFileName ? file.FileName : Guid.NewGuid() + file.FileName[file.FileName.LastIndexOf(".")..];
            Stream = file.OpenReadStream();
        }

        public CreateFileTableDto(Stream stream, string fileName)
        {
            FileName = fileName;
            Stream = stream;
        }
    }
}
