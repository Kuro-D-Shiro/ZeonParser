using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using ZeonParser.Parser.Interfaces;
using ZeonParser.Parser.Settings;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace ZeonParser.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestController(IHtmlLoader okak, IOptions<ZeonParserSettings> option) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            var HTMLString = await okak.LoadPageByURL(option.Value.Url);

            HtmlParser htmlParser = new();
            IHtmlDocument doc = htmlParser.ParseDocument(HTMLString);
            List<IElement> elements = doc.QuerySelectorAll(".catalog-menu-list-one").ToList();

            var aboba = elements[7].TextContent;

            return Ok();
        }
    }
}
