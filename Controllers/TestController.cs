using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZeonService.Models;
using ZeonService.Parser;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController(IHtmlLoader okak, IOptions<ZeonParserSettings> option, IZeonParser zeonParser) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            var mainCategoryElement = await ZeonPage.TryCreate(await okak.LoadPageByURL("https://zeon18.ru/zeon/"));
            var els = mainCategoryElement.GetElementsBySelector(".catalog-index-cell");
            
            await zeonParser.ParseDFS();
            /*var page = await ZeonPage.TryCreate(HTMLString);
            if (page == null)
            {
                return StatusCode(500);
            }
            await Parse(page, ".catalog-menu-list-one");*/

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
        Task Parse();
        Task ParseDFS();
    }

    public class ZeonParser(IHtmlLoader htmlLoader, IOptions<ZeonParserSettings> options) : IZeonParser
    {
        private readonly IHtmlLoader _htmlLoader = htmlLoader;
        private readonly ZeonParserSettings _parserSettings = options.Value;
        private readonly string _mainCategoriesSelector = ".catalog-menu-list-one";

        /*public async Task<T> RecursiveParse<T>(string selector)
        {

        }*/

        public async Task ParseDFS()
        {
            var mainPage = await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(_parserSettings.Url));

            var mainCategoryElements = mainPage?.GetElementsBySelector(_mainCategoriesSelector).Skip(5)
                ?? throw new Exception("Не удалось создать страницу по HTML."); //nullable ???

            foreach (var mainCategoryElement in mainCategoryElements)
            {
                var categoryParser = new ZeonCategoryParser(mainCategoryElement);
                var currentMainCategory = categoryParser.Parse(null);

                var mainCategoryPage = await ZeonPage.TryCreate(currentMainCategory.Link); //nullable ???

                //использовать второй стек для категорий, чтоб из привязывать к дочерним параллельно парсингу !!!
                Stack<ZeonPage> zeonPages = new Stack<ZeonPage>();
                var el = await ZeonPage.TryCreate(await _htmlLoader.LoadPageByURL(currentMainCategory.Link)); //тоже переделать
                zeonPages.Push(el/*mainCategoryPage*/); //nullable ???
                Stack<Category> categories = new Stack<Category>();
                categories.Push(currentMainCategory);
                Category currentSubcategory;

                while (zeonPages.Count > 0)
                {
                    var page = zeonPages.Pop();
                    var parentCategory = categories.Pop();

                    var subcategoryIndexCells = page.GetElementsBySelector(".catalog-index-cell");

                    if(!subcategoryIndexCells.Any())
                    {
                        var els = page.GetElementsBySelector(".catalog-grid-cell");
                    }

                    foreach (var subcategoryIndexCell in subcategoryIndexCells)
                    {
                        categoryParser = new ZeonCategoryParser(subcategoryIndexCell);
                        currentSubcategory = categoryParser.Parse(parentCategory);
                        categories.Push(currentSubcategory);

                        el = await ZeonPage.TryCreate(parentCategory.Link); //переделать ес чо
                        zeonPages.Push(el); //nullable ???
                    }
                }
            }    
        }

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
                var currentMainCategory = categoryParser.Parse(null);

                var mainCategoryPage = await ZeonPage.TryCreate(currentMainCategory.Link);

                var subcategoryIndexCells = mainCategoryPage.GetElementsBySelector(".catalog-index-cell");
                foreach (var subcategoryIndexCell in subcategoryIndexCells)
                {
                    while (subcategoryIndexCells.Any())
                    {
                        Category currentSubcategory;
                        /*foreach (var subcategoryIndexCell in subcategoryIndexCells)
                        {
                            categoryParser = new ZeonCategoryParser(subcategoryIndexCell);
                            currentSubcategory = categoryParser.Parse(currentMainCategory);
                        }*/
                    }
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
                ?? throw new Exception("У блока категории не нашлось ссылки на неё."); //nullable ???
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
