using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class CategoryRepository(IDbContextFactory<ZeonDbContext> zeonDbContextFacory) : ICategoryRepository
    {
        private readonly IDbContextFactory<ZeonDbContext> zeonDbContextFacory = zeonDbContextFacory;

        public async Task<IEnumerable<Category>> GetAll()
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Categories
                    .FromSqlRaw("select * from categories").ToListAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetMainCategories()
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Categories
                    .FromSqlRaw("select * from categories where parent_category_id is null")
                    .ToListAsync();
            }
        }

        public async Task<Category?> GetById(long id)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Categories
                    .FromSqlRaw("select * from categories where category_id = {0}", id).FirstOrDefaultAsync();
            }
        }

        public async Task<Category> GetByName(string name)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Categories
                    .FromSqlRaw("select * from categories where name = {0}", name).FirstAsync();
            }
        }

        public async Task<IEnumerable<Category>> GetAllByCategoryId(long categoryId)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                return await zeonDbContext.Categories
                    .FromSqlRaw("select cc.* from categories pc" +
                    " join categories cc" +
                    " on pc.category_id = cc.parent_category_id" +
                    " where pc.category_id = {0}", categoryId)
                    .ToListAsync();
            }
        }

        public async Task<long> Create(Category item)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                long? categoryId = (await zeonDbContext.Categories
                    .FromSqlRaw("select * from categories where link = {0}", item.Link).FirstOrDefaultAsync())?.CategoryId;
                if (categoryId == null)
                {
                    return zeonDbContext.Database
                        .SqlQueryRaw<long>("insert into categories (name, link, parent_category_id)" +
                        " values ({0}, {1}, {2}) returning category_id",
                        item.Name, item.Link, item.ParentCategoryId)
                        .AsEnumerable()
                        .First();
                }
                else return categoryId.Value;
            }
        }

        public async Task Update(Category item)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                await zeonDbContext.Database
                    .ExecuteSqlRawAsync("update categories set" +
                    "name = {0} where link = {1}", item.Name, item.Link);
            }
        }

        public async Task Delete(int id)
        {
            using (var zeonDbContext = zeonDbContextFacory.CreateDbContext())
            {
                await zeonDbContext.Database
                    .ExecuteSqlRawAsync("delete from categories where category_id = {0}", id);
            }
        }
    }
}
