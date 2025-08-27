using ZeonService.Models;

namespace ZeonService.Parser.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetMainCategories();
    }
}
