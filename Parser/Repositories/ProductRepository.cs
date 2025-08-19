using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class ProductRepository(ZeonDbContext zeonDbContext) : IProductRepository
    { 
        private readonly ZeonDbContext zeonDbContext = zeonDbContext;

        public IEnumerable<Product> GetAll()
        {
            return zeonDbContext.Products
                .FromSqlRaw("select * from products");
        }

        public async Task<Product> GetById(int id)
        {
            return await zeonDbContext.Products
                .FromSqlRaw("select * from products where product_id = {0}", id).FirstAsync();
        }

        public async Task<Product> GetByName(string name)
        {
            return await zeonDbContext.Products
                .FromSqlRaw("select * from products where name = {0}", name).FirstAsync();
        }

        public async Task<bool> IsExists(string link)
        {
            return await zeonDbContext.Products
                .FromSqlRaw("select * from products where link = {0}", link).AnyAsync();
        }

        public async Task<IEnumerable<Product>> GetAllByName(string name)
        {
            return await zeonDbContext.Products
                .FromSqlRaw("select p.* from products p" +
                " left join categories c" +
                " on p.category_id = c.category_id" +
                " where p.name like {0}", $"%{name}%")
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllByCategoryId(long categoryId)
        {
            
        }

        public async Task<long> Create(Product item)
        {
            if (!await IsExists(item.Link))
            {
                return zeonDbContext.Database
                    .SqlQueryRaw<long>("insert into products (name, link, image_path, old_price," +
                    " current_price, description, updated_at, category_id)" +
                    " values ({0}, {1}, {2}, {3}, {4}, {5}, CURRENT_TIMESTAMP, {6}) returning product_id",
                    item.Name, item.Link, item.ImagePath, item.OldPrice,
                    item.CurrentPrice, item.Description, item.CategoryId)
                    .AsEnumerable()
                    .First();
            }
            else return -1;
        }

        public async Task Delete(int id)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("delete from products where product_id = {0}", id);
        }

        public async Task Update(Product item)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("update products " +
                "set name = {0}, old_price = {1}, " +
                "current_price = {2}, description = {3}, " +
                "updated_at = CURRENT_TIMESTAMP, category_id = {4} " +
                "where link = {5}", item.Name, item.OldPrice, item.CurrentPrice,
                item.Description, item.CategoryId, item.Link);
        }
    }
}
