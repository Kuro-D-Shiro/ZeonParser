using AngleSharp.Dom;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    class ZeonCategoryParser : IZeonCategoryParser
    {
        public async Task<Category> Parse(IElement categoryElement, long? parentCategoryId)
        {
            var category = new Category();
            category.Name = categoryElement.TextContent.Trim();
            category.ParentCategoryId = parentCategoryId;
            category.Link = categoryElement.QuerySelector("a")?.GetAttribute("href")
                ?? throw new Exception("У блока категории не нашлось ссылки на неё."); //nullable ???
            return category;
        }
    }
}
