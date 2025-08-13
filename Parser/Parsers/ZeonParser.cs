using Microsoft.Extensions.Options;
using ZeonService.Models;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Parsers
{
    public class ZeonParser(IHtmlLoader htmlLoader,
        IOptions<ZeonParserSettings> options,
        IZeonCategoryParser zeonCategoryParser,
        IZeonProductParser zeonProductParser,
        IRepository<Category> catgoryRepository,
        IRepository<Product> productRepository) : IZeonParser
    {
        private readonly IHtmlLoader htmlLoader = htmlLoader;
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IZeonCategoryParser zeonCategoryParser = zeonCategoryParser;
        private readonly IZeonProductParser zeonProductParser = zeonProductParser;
        private readonly IRepository<Category> catgoryRepository = catgoryRepository;
        private readonly IRepository<Product> productRepository = productRepository;

        public async Task Parse()
        {
            var mainPage = await ZeonPage.TryCreate(await htmlLoader.Download(parserSettings.Url));
            var mainCategoryElements = mainPage.GetElementsBySelector(parserSettings.Selectors.MainCategorySelector).Skip(5);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var currentMainCategory = await zeonCategoryParser.Parse(mainCategoryElement, null);

                await catgoryRepository.Create(currentMainCategory);

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

                    var subcategoryIndexCells = page.GetElementsBySelector(parserSettings.Selectors.SubcategorySelector);

                    if (!subcategoryIndexCells.Any())
                    {
                        var productsGridCells = page.GetElementsBySelector(parserSettings.Selectors.ProductSelector);
                        foreach (var productsGridCell in productsGridCells)
                        {
                            var product = await zeonProductParser.Parse(productsGridCell, parentCategory);
                            await productRepository.Create(product);
                            await Task.Delay(2000);
                        }
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        currentSubcategory = await zeonCategoryParser.Parse(subcategoryIndexCell, parentCategory);
                        categories.Push(currentSubcategory);
                        await catgoryRepository.Create(currentSubcategory);

                        zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentSubcategory.Link)));
                        await Task.Delay(2000);
                    }
                }
                await Task.Delay(20000);
            }
        }
    }
}
