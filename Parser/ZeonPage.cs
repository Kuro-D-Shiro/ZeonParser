using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ZeonService.Parser
{
    class ZeonPage(IHtmlDocument htmlDocument)
    {
        private readonly IHtmlDocument htmlDocument = htmlDocument;

        public IEnumerable<IElement> GetElementsBySelector(string selector)
        {
            return htmlDocument.QuerySelectorAll(selector);
        }

        public static async Task<ZeonPage> TryCreate(string pageSource)
        {
            HtmlParser htmlParser = new();
            var doc = await htmlParser.ParseDocumentAsync(pageSource);

            return new ZeonPage(doc);
        }
    }
}
