using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    class ZeonCategoryParser : IZeonCategoryParser
    {
        public Task<Category> Parse(IElement categoryElement, Category? parentCategory)
        {
            var category = new Category();
            category.Name = categoryElement.TextContent;
            category.ParentCategory = parentCategory;
            category.ParentCategoryId = parentCategory?.CategoryId
                ?? null;
            category.Link = categoryElement.QuerySelector("a")?.GetAttribute("href")
                ?? throw new Exception("У блока категории не нашлось ссылки на неё."); //nullable ???
            return Task.FromResult(category);
        }
    }
}
