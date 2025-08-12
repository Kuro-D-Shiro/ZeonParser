using Microsoft.Extensions.Options;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Parsers
{
    public class ZeonParser(IHtmlLoader htmlLoader,
        IOptions<ZeonParserSettings> options,
        IZeonCategoryParser zeonCategoryParser,
        IZeonProductParser zeonProductParser) : IZeonParser
    {
        private readonly IHtmlLoader htmlLoader = htmlLoader;
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IZeonCategoryParser zeonCategoryParser = zeonCategoryParser;
        private readonly IZeonProductParser zeonProductParser = zeonProductParser;
        private readonly string mainCategoriesSelector = ".catalog-menu-list-one";

        public async Task Parse()
        {
            var mainPage = await ZeonPage.TryCreate(await htmlLoader.Download(parserSettings.Url));
            var mainCategoryElements = mainPage.GetElementsBySelector(mainCategoriesSelector).Skip(5);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var currentMainCategory = await zeonCategoryParser.Parse(mainCategoryElement, null);

                var mainCategoryPage = await ZeonPage.TryCreate(currentMainCategory.Link);

                Stack<ZeonPage> zeonPages = [];
                zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentMainCategory.Link)));
                Stack<Category> categories = [];
                categories.Push(currentMainCategory);
                Category currentSubcategory;

                while (zeonPages.Count > 0)
                {
                    var page = zeonPages.Pop();
                    var parentCategory = categories.Pop();

                    var subcategoryIndexCells = page.GetElementsBySelector(".catalog-index-cell");

                    if (!subcategoryIndexCells.Any())
                    {
                        var productsGridCells = page.GetElementsBySelector(".catalog-grid-cell");
                        foreach (var productsGridCell in productsGridCells)
                        {
                            var okak = zeonCategoryParser.Parse(productsGridCell, parentCategory);
                        }
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        currentSubcategory = await zeonCategoryParser.Parse(subcategoryIndexCell, parentCategory);
                        categories.Push(currentSubcategory);

                        zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentSubcategory.Link)));
                    }
                }
            }
        }
    }
}
