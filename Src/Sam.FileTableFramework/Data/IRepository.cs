using Sam.FileTableFramework.Data.Dto;
using Sam.FileTableFramework.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sam.FileTableFramework.Data
{
    public interface IRepository
    {
        Task<string> CreateAsync(CreateFileEntityDto model);
        Task<FileEntity> FindByNameAsync(string fileName);
        Task<IEnumerable<FileEntityDto>> GetAllAsync();
        Task<PagedListFileEntityDto> GetPagedListAsync(int page, int pageCount);
        Task<int> RemoveByNameAsync(string fileName);
        Task<int> Count();
    }
}