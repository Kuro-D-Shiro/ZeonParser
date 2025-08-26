using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
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
        ICategoryRepository categoryRepository,
        IProductRepository productRepository) : IZeonParser
    {
        private readonly IHtmlLoader htmlLoader = htmlLoader;
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IZeonCategoryParser zeonCategoryParser = zeonCategoryParser;
        private readonly IZeonProductParser zeonProductParser = zeonProductParser;
        private readonly IRepository<Category> categoryRepository = categoryRepository;
        private readonly IRepository<Product> productRepository = productRepository;
        private readonly HtmlParser htmlParser = new();

        public async Task Parse()
        {
            var mainPageDoc = await LoadDocument(parserSettings.Url);
            var mainCategoryElements = SelectMainCategories(mainPageDoc);

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var mainCategory = await zeonCategoryParser.Parse(mainCategoryElement, null);
                var mainCategoryId = await categoryRepository.Create(mainCategory);

                var firstPage = await LoadDocument(mainCategory.Link);
                var pageStack = new Stack<IHtmlDocument>();
                var categoryStack = new Stack<long>();

                pageStack.Push(firstPage);
                categoryStack.Push(mainCategoryId);

                await ProcessPagesRecursively(pageStack, categoryStack);
            }
        }

        private async Task ProcessPagesRecursively(Stack<IHtmlDocument> pages, Stack<long> categories)
        {
            while (pages.Count > 0)
            {
                var page = pages.Pop();
                var parentCategoryId = categories.Pop();

                var subcategoryCells = page.QuerySelectorAll(parserSettings.Selectors.Subcategory);
                if (!subcategoryCells.Any())
                    await ProcessProductListings(page, parentCategoryId);

                foreach (var cell in subcategoryCells)
                {
                    var subcategory = await zeonCategoryParser.Parse(cell, parentCategoryId);
                    var subcategoryId = await categoryRepository.Create(subcategory);

                    var subPage = await LoadDocument(subcategory.Link);
                    pages.Push(subPage);
                    categories.Push(subcategoryId);
                }
            }
        }

        private async Task ProcessProductListings(IHtmlDocument page, long parentCategoryId)
        {
            await ProcessProductsOnPage(page, parentCategoryId);

            var paginationLinks = page
                .QuerySelectorAll(parserSettings.Selectors.Pagination)
                .Select(a => a.GetAttribute("href"))
                .Where(href => !string.IsNullOrEmpty(href));

            foreach (var link in paginationLinks)
            {
                var pagedDoc = await LoadDocument(link);
                await ProcessProductsOnPage(pagedDoc, parentCategoryId);
            }
        }

        private async Task ProcessProductsOnPage(IHtmlDocument page, long parentCategoryId)
        {
            var productLinks = page
                .QuerySelectorAll(parserSettings.Selectors.ProductLink)
                .Select(el => el.GetAttribute("href"))
                .Where(href => !string.IsNullOrEmpty(href));

            foreach (var productLink in productLinks)
            {
                var productDoc = (await LoadDocument(productLink)).DocumentElement;
                var product = await zeonProductParser.Parse(productDoc, parentCategoryId);
                var newId = await productRepository.Create(product);
                if (newId == -1)
                    await productRepository.Update(product);
            }
        }

        private async Task<IHtmlDocument> LoadDocument(string url)
        {
            var content = await htmlLoader.Download(url);
            return await htmlParser.ParseDocumentAsync(content);
        }

        private List<IElement> SelectMainCategories(IHtmlDocument doc)
        {
            var nodes = doc.QuerySelectorAll(parserSettings.Selectors.MainCategory).Skip(2).ToList();
            nodes.RemoveRange(1, 2);
            return nodes;
        }
    }
}