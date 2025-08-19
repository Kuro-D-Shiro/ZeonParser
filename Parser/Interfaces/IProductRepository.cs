using ZeonService.Models;

namespace ZeonService.Parser.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<bool> IsExists(string link);
        Task<IEnumerable<Product>> GetAllByCategoryId(long categoryId);
    }
}
