using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
        private readonly HtmlParser htmlParser = new();

        public async Task Parse()
        {
            var mainPage = await htmlParser.ParseDocumentAsync(await htmlLoader.Download(parserSettings.Url));
            var mainCategoryElements = mainPage.QuerySelectorAll(parserSettings.Selectors.MainCategory).Skip(5);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var currentMainCategory = await zeonCategoryParser.Parse(mainCategoryElement, null);

                var currentMainCategoryId = await catgoryRepository.Create(currentMainCategory);

                var mainCategoryPage = await htmlParser.ParseDocumentAsync(await htmlLoader.Download(currentMainCategory.Link));

                Stack <IHtmlDocument> zeonPages = [];
                zeonPages.Push(mainCategoryPage);
                Stack<long> categories = [];
                categories.Push(currentMainCategoryId);
                Category currentSubcategory;

                while (zeonPages.Count > 0)
                {
                    var page = zeonPages.Pop();
                    var parentCategoryId = categories.Pop();

                    var subcategoryIndexCells = page.QuerySelectorAll(parserSettings.Selectors.Subcategory);

                    if (!subcategoryIndexCells.Any())
                    {
                        var productCardLinks = page.QuerySelectorAll(parserSettings.Selectors.ProductLink)
                            .Select(el => el.GetAttribute("href"));

                        IElement productCatd;
                        foreach (var productCardLink in productCardLinks)
                        {
                            productCatd = (await htmlParser.ParseDocumentAsync
                                (await htmlLoader.Download(productCardLink)))
                                .DocumentElement;
                            var product = await zeonProductParser.Parse(productCatd, parentCategoryId);
                            if (await productRepository.Create(product) == -1)
                                await productRepository.Update(product);
                            //await Task.Delay(500);
                        }

                        var paginationPageLinks = page.QuerySelectorAll("span.paginator-pages a")
                            .Select(el => el.GetAttribute("href"));

                        foreach (var paginationPageLink in paginationPageLinks)
                        {
                            var paginationPage = await htmlParser.ParseDocumentAsync(await htmlLoader.Download(paginationPageLink));

                            productCardLinks = paginationPage.QuerySelectorAll(parserSettings.Selectors.ProductLink)
                                .Select(el => el.GetAttribute("href"));
                            foreach (var productCardLink in productCardLinks)
                            {
                                productCatd = (await htmlParser.ParseDocumentAsync
                                    (await htmlLoader.Download(productCardLink)))
                                    .DocumentElement;
                                var product = await zeonProductParser.Parse(productCatd, parentCategoryId);
                                if (await productRepository.Create(product) == -1)
                                    await productRepository.Update(product);
                                //await Task.Delay(500);
                            }
                        }
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        currentSubcategory = await zeonCategoryParser.Parse(subcategoryIndexCell, parentCategoryId);
                        var currentSubcategoryId = await catgoryRepository.Create(currentSubcategory);
                        categories.Push(currentSubcategoryId);

                        zeonPages.Push(await htmlParser.ParseDocumentAsync(await htmlLoader.Download(currentSubcategory.Link)));
                        //await Task.Delay(500);
                    }
                }
                //await Task.Delay(500);
            }
        }
    }
}
