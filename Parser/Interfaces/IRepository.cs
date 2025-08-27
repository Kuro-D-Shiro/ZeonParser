using ZeonService.Models;

namespace ZeonService.Parser.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(long id);
        Task<T> GetByName(string name);
        Task<IEnumerable<T>> GetAllByCategoryId(long categoryId);
        Task<long> Create(T item);
        Task Update(T item);
        Task Delete(int id);
    }
}
