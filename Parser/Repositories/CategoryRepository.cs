using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ZeonService.Data;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Repositories
{
    public class CategoryRepository(ZeonDbContext zeonDbContext) : IRepository<Category>
    {
        private readonly ZeonDbContext zeonDbContext = zeonDbContext;

        public async Task<IEnumerable<Category>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Category> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> GetByName(string name)
        {
            return await zeonDbContext.Categories
                .FromSqlRaw("select * from categories where name = {0}", name).FirstAsync();
        }

        public async Task Create(Category item)
        {
            await zeonDbContext.Database
                .ExecuteSqlRawAsync("insert into categories (name, link, parent_category_id) values ({0}, {1}, {2})",
                item.Name, item.Link, item.ParentCategoryId);
        }

        public async Task Update(Category item)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
