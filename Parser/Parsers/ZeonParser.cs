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
        ICategoryRepository catgoryRepository,
        IProductRepository productRepository) : IZeonParser
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

                var currentMainCategoryId = await catgoryRepository.Create(currentMainCategory);

                var mainCategoryPage = await ZeonPage.TryCreate(currentMainCategory.Link);

                Stack<ZeonPage> zeonPages = [];
                zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentMainCategory.Link)));
                Stack<long> categories = [];
                categories.Push(currentMainCategoryId);
                Category currentSubcategory;

                while (zeonPages.Count > 0)
                {
                    var page = zeonPages.Pop();
                    var parentCategoryId = categories.Pop();

                    var subcategoryIndexCells = page.GetElementsBySelector(parserSettings.Selectors.SubcategorySelector);

                    if (!subcategoryIndexCells.Any())
                    {
                        var productsGridCells = page.GetElementsBySelector(parserSettings.Selectors.ProductSelector);
                        foreach (var productsGridCell in productsGridCells)
                        {
                            var product = await zeonProductParser.Parse(productsGridCell, parentCategoryId);
                            if (await productRepository.Create(product) == -1)
                                await productRepository.Update(product);
                            //await Task.Delay(500);
                        }
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        currentSubcategory = await zeonCategoryParser.Parse(subcategoryIndexCell, parentCategoryId);
                        var currentSubcategoryId = await catgoryRepository.Create(currentSubcategory);
                        categories.Push(currentSubcategoryId);

                        zeonPages.Push(await ZeonPage.TryCreate(await htmlLoader.Download(currentSubcategory.Link)));
                        //await Task.Delay(500);
                    }
                }
                //await Task.Delay(500);
            }
        }
    }
}
