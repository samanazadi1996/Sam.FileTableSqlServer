namespace Sam.EndPoint.WebApi.Models
{
    public class FileEntityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
    }
}
