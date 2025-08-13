using Microsoft.EntityFrameworkCore;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class ProductRepository(ZeonDbContext zeonDbContext) : IRepository<Product>
    {
        private readonly ZeonDbContext zeonDbContext = zeonDbContext;

        public async Task<IEnumerable<Product>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> GetByName(string name)
        {
            return await zeonDbContext.Products
                .FromSqlRaw("select * from products where name = '{0}'", name).FirstAsync();
        }

        public async Task Create(Product item)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("insert into products (name, link, image_path, old_price, current_price, in_stock, category_id)" +
                " values ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                item.Name, item.Link, item.ImagePath, item.OldPrice, item.CurrentPrice, item.InStock, item.CategoryId);
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Product item)
        {
            throw new NotImplementedException();
        }
    }
}
