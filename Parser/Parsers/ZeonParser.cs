using Microsoft.Extensions.Options;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Parsers
{
    public class ZeonParser(IHtmlLoader htmlLoader, IOptions<ZeonParserSettings> options) : IZeonParser
    {
        private readonly IHtmlLoader _htmlLoader = htmlLoader;
        private readonly ZeonParserSettings _parserSettings = options.Value;
        private readonly string _mainCategoriesSelector = ".catalog-menu-list-one";

        /*public async Task<T> RecursiveParse<T>(string selector)
        {

        }*/

        public async Task Parse()
        {
            var mainPage = await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(_parserSettings.Url));

            var mainCategoryElements = mainPage.GetElementsBySelector(_mainCategoriesSelector).Skip(5);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var categoryParser = new ZeonCategoryParser(mainCategoryElement);
                var currentMainCategory = categoryParser.Parse(null);

                var mainCategoryPage = await ZeonPage.TryCreate(currentMainCategory.Link);

                Stack<ZeonPage> zeonPages = new Stack<ZeonPage>();
                zeonPages.Push(await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(currentMainCategory.Link)));
                Stack<Category> categories = new Stack<Category>();
                categories.Push(currentMainCategory);
                Category currentSubcategory;

                while (zeonPages.Count > 0)
                {
                    var page = zeonPages.Pop();
                    var parentCategory = categories.Pop();

                    var subcategoryIndexCells = page.GetElementsBySelector(".catalog-index-cell");

                    if (!subcategoryIndexCells.Any())
                    {
                        //логика парсинга товара
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        categoryParser = new ZeonCategoryParser(subcategoryIndexCell);
                        currentSubcategory = categoryParser.Parse(parentCategory);
                        categories.Push(currentSubcategory);

                        zeonPages.Push(await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(currentSubcategory.Link)));
                    }
                }
            }
        }
    }
}
