using Microsoft.Extensions.Options;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Parsers
{
    public class ZeonParser(IHtmlLoader htmlLoader,
        IOptions<ZeonParserSettings> options,
        IDownloadAndSaveImageService downloadAndSaveImageService) : IZeonParser
    {
        private readonly IHtmlLoader htmlLoader = htmlLoader;
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IDownloadAndSaveImageService downloadAndSaveImageService = downloadAndSaveImageService;
        private readonly string mainCategoriesSelector = ".catalog-menu-list-one";

        /*public async Task<T> RecursiveParse<T>(string selector)
        {

        }*/

        public async Task Parse()
        {
            ZeonCategoryParser categoryParser;
            ZeonProductParser productParser;
            var mainPage = await ZeonPage.TryCreate(await htmlLoader.Download(parserSettings.Url));

            var mainCategoryElements = mainPage.GetElementsBySelector(mainCategoriesSelector).Skip(5);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                categoryParser = new ZeonCategoryParser(mainCategoryElement);
                var currentMainCategory = await categoryParser.Parse(null);

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
                            productParser = new ZeonProductParser(productsGridCell, downloadAndSaveImageService);
                            var okak = productParser.Parse(parentCategory);
                        }
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        categoryParser = new ZeonCategoryParser(subcategoryIndexCell);
                        currentSubcategory = await categoryParser.Parse(parentCategory);
                        categories.Push(currentSubcategory);

                        zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentSubcategory.Link)));
                    }
                }
            }
        }
    }
}
