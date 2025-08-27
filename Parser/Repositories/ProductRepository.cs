using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class ProductRepository(IDbContextFactory<ZeonDbContext> zeonDbContextFacory) : IProductRepository
    {
        private readonly IDbContextFactory<ZeonDbContext> zeonDbContextFacory = zeonDbContextFacory;

        public async Task<IEnumerable<Product>> GetAll()
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products")
                    .Include(p => p.Category)
                    .ToListAsync();
            }
        }

        public async Task<Product?> GetById(long id)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products where product_id = {0}", id)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<Product> GetByName(string name)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products where name = {0}", name).FirstAsync();
            }
        }

        public async Task<bool> IsExists(string link)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products where link = {0}", link).AnyAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllByName(string name)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products" +
                    " where name ilike {0}", $"%{name}%")
                    .Include(p => p.Category)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllByCategoryId(long categoryId)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Products
                    .FromSqlRaw("select * from products where category_id = {0}", categoryId)
                    .ToListAsync();
            }
        }

        public async Task<long> Create(Product item)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                var specsParam = new NpgsqlParameter("p_specs", NpgsqlDbType.Jsonb)
                {
                    Value = item.Specifications ?? (object)DBNull.Value
                };
                await zeonDbContext.Database
                    .ExecuteSqlRawAsync("insert into products (name, link, image_path, old_price," +
                    " current_price, specifications, updated_at, category_id)" +
                    " values ({0}, {1}, {2}, {3}, {4}, @p_specs, CURRENT_TIMESTAMP, {6})",
                    item.Name, item.Link, item.ImagePath, item.OldPrice,
                    item.CurrentPrice, specsParam, item.CategoryId);
                return default;
            }
        }

        public async Task Delete(int id)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                await zeonDbContext.Database
                    .ExecuteSqlRawAsync("delete from products where product_id = {0}", id);
            }
        }

        public async Task Update(Product item)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                var specsParam = new NpgsqlParameter("p_specs", NpgsqlDbType.Jsonb)
                {
                    Value = item.Specifications ?? (object)DBNull.Value
                };
                await zeonDbContext.Database
                    .ExecuteSqlRawAsync("update products " +
                    "set name = {0}, old_price = {1}, " +
                    "current_price = {2}, specifications = @p_specs, " +
                    "updated_at = CURRENT_TIMESTAMP, category_id = {4} " +
                    "where link = {5}", item.Name, item.OldPrice, item.CurrentPrice,
                    specsParam, item.CategoryId, item.Link);
            }
        }
    }
}
