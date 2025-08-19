namespace ZeonService.Parser.Settings
{
    public class ZeonParserSettings
    {
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public Selectors Selectors { get; set; }
    }

    public class Selectors
    {
        public string MainCategorySelector { get; set; }
        public string SubcategorySelector { get; set; }
        public string ProductLinkSelector { get; set; }
    }
}
