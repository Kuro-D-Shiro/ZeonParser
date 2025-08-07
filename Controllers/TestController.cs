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
            await zeonParser.Parse();
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

        public static async Task<ZeonPage> TryCreate(string pageSource)
        {
            HtmlParser htmlParser = new();
            var doc = await htmlParser.ParseDocumentAsync(pageSource);

            return new ZeonPage(doc);
        }
    }
