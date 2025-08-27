using AngleSharp.Dom;
using FluentResults;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Parsers
{
    class ZeonCategoryParser(ILogger<ZeonCategoryParser> logger) : IZeonCategoryParser
    {
        private readonly ILogger<ZeonCategoryParser> logger = logger;
        public async Task<Category?> Parse(IElement categoryElement, long? parentCategoryId)
        {
            var name = GetCategoryName(categoryElement);
            if (name == null)
            {
                logger.LogError("Не удалось получить имя категории со страницы.");
                return null; 
            }

            var link = GetCategoryLink(categoryElement);
            if (link == null)
            {
                logger.LogError("Не удалось получить ссылку на категорию со страницы.");
                return null;
            }

            return new Category
            {
                Name = name,
                Link = link,
                ParentCategoryId = parentCategoryId
            };
        }
        
        private string? GetCategoryName(IElement categoryElement)
        {
            return categoryElement.TextContent.Trim() 
                ?? null;
        }

        private string? GetCategoryLink(IElement categoryElement)
        {
            return categoryElement.QuerySelector("a")?.GetAttribute("href")
                ?? null;
        }
    }
}
