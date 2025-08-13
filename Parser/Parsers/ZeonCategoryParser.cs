using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    class ZeonCategoryParser(IRepository<Category> repository) : IZeonCategoryParser
    {
        public async Task<Category> Parse(IElement categoryElement, Category? parentCategory)
        {
            if (parentCategory != null)
            {
                parentCategory = await repository.GetByName(parentCategory.Name);
            }

            var category = new Category();
            category.Name = categoryElement.TextContent.Trim();
            category.ParentCategory = parentCategory;
            category.ParentCategoryId = parentCategory?.CategoryId
                ?? null;
            category.Link = categoryElement.QuerySelector("a")?.GetAttribute("href")
                ?? throw new Exception("У блока категории не нашлось ссылки на неё."); //nullable ???
            return category;
        }
    }
}
