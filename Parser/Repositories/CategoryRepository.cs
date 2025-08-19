using Microsoft.EntityFrameworkCore;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class CategoryRepository(ZeonDbContext zeonDbContext) : ICategoryRepository
    {
        private readonly ZeonDbContext zeonDbContext = zeonDbContext;

        public IEnumerable<Category> GetAll()
        {
            return zeonDbContext.Categories
                .FromSqlRaw("select * from categories").AsEnumerable();
        }

        public async Task<Category> GetById(int id)
        {
            return await zeonDbContext.Categories
                .FromSqlRaw("select * from categories where category_id = {0}", id).FirstAsync();
        }

        public async Task<Category> GetByName(string name)
        {
            return await zeonDbContext.Categories
                .FromSqlRaw("select * from categories where name = {0}", name).FirstAsync();
        }

        public async Task<IEnumerable<Category>> GetAllByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<long> Create(Category item)
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

        public async Task Update(Category item)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("update categories set" +
                "name = {0} where link = {1}", item.Name, item.Link);
        }

        public async Task Delete(int id)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("delete from categories where category_id = {0}", id);
        }
    }
}
