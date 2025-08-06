using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using ZeonService.Models;

namespace ZeonService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController(IHtmlLoader okak, IOptions<ZeonParserSettings> option) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            var HTMLString = await okak.LoadPageByURL(option.Value.Url);

            var page = await ZeonPage.TryCreate(HTMLString);
            if (page == null)
            {
                return StatusCode(500);
            }
            await Parse(page, ".catalog-menu-list-one");

            return Ok();
        }

        private async Task Parse(ZeonPage page, string selector)
        {
            List<IElement> elements = page.GetElementsBySelector(selector).ToList();
            if (selector == ".catalog-grid-cell")
                return;
            foreach (var element in elements)
            {
                var url = element.QuerySelector("a")?.GetAttribute("href");
                var newPage = await ZeonPage.TryCreate(await okak.LoadPageByURL(option.Value.Url));
                await Parse(newPage, ".catalog-index-cell");
            }
        }
    }

    public interface IZeonParser
    {
        public Task Parse();
    }

    public class ZeonParser(IHtmlLoader htmlLoader, IOptions<ZeonParserSettings> options) : IZeonParser
    {
        private readonly IHtmlLoader _htmlLoader = htmlLoader;
        private readonly ZeonParserSettings _parserSettings = options.Value;
        private readonly string _mainCategoriesSelector = ".catalog-menu-list-one";

        public async Task Parse()
        {
            var mainPage = await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(_parserSettings.Url));

            var mainCategoryElements = mainPage?.GetElementsBySelector(_mainCategoriesSelector)
                ?? throw new Exception("Не удалось создать страницу по HTML.");
            /*
                            var tasksForCreateZeonPages = mainCategoryElements
                                .Select(x => ZeonPage.TryCreate(x.QuerySelector("a")?.GetAttribute("href")))
                                .ToList();
                            List<ZeonPage> mainCategoryPages = (await Task.WhenAll(tasksForCreateZeonPages))
                                .Where(p => p != null) 
                                .ToList();*/

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var categoryParser = new ZeonCategoryParser(mainCategoryElement);
                var mainCategory = categoryParser.Parse(null);

                var mainCategoryPage = await ZeonPage.TryCreate(mainCategory.Link);

                var subcategoryIndexCells = mainCategoryPage.GetElementsBySelector(".catalog-index-cell");
                while (subcategoryIndexCells.Any())
                {

                }
            }
        }
    }

    class ZeonCategoryParser(IElement categoryElement)
    {
        private readonly IElement _categoryElement = categoryElement;

        public Category Parse(Category? parentCategory)
        {
            var category = new Category();
            category.Name = _categoryElement.TextContent;
            category.ParentCategory = parentCategory;
            category.ParentCategoryId = parentCategory?.CategoryId
                ?? null;
            category.Link = _categoryElement.QuerySelector("a")?.GetAttribute("href")
                ?? throw new Exception("У блока категории не нашлось ссылки на неё.");
            return category;
        }
    }



    class ZeonPage(IHtmlDocument htmlDocument)
    {
        private readonly IHtmlDocument _htmlDocument = htmlDocument;

        public IEnumerable<IElement> GetElementsBySelector(string selector)
        {
            return _htmlDocument.QuerySelectorAll(selector);
        }

        public static async Task<ZeonPage?> TryCreate(string pageSource)
        {
            HtmlParser htmlParser = new();
            var doc = await htmlParser.ParseDocumentAsync(pageSource);

            if (doc is null)
                return null;

            return new ZeonPage(doc);
        }
    }
}

/*HtmlParser htmlParser = new();
            IHtmlDocument doc = htmlParser.ParseDocument(HTMLString);
            List<IElement> elements = doc.QuerySelectorAll(".catalog-menu-list-one").ToList();
            var aboba = elements[7].QuerySelector("a")?.GetAttribute("href");

            HTMLString = await okak.LoadPageByURL(aboba);
            doc = htmlParser.ParseDocument(HTMLString);
            elements = doc.QuerySelectorAll(".catalog-index-cell").ToList();
            aboba = elements[7].QuerySelector(".catalog-index-catalog-title")?.GetAttribute("href");

            HTMLString = await okak.LoadPageByURL(aboba);
            doc = htmlParser.ParseDocument(HTMLString);
            elements = doc.QuerySelectorAll(".catalog-index-cell").ToList();
            aboba = elements[7].QuerySelector(".catalog-index-catalog-title")?.GetAttribute("href");*/
