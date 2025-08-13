namespace ZeonService.Parser.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> GetByName(string name);
        Task Create(T item);
        Task Update(T item);
        Task Delete(int id);
    }
}
