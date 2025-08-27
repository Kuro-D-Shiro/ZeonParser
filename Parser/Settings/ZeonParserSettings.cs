namespace ZeonService.Parser.Settings
{
    public class ZeonParserSettings
    {
        public string Url { get; set; }
        public string UserAgent { get; set; }
        public int TimeoutBetweenRequestsMilliseconds { get; set; }
        public Selectors Selectors { get; set; }
    }

    public class Selectors
    {
        public string MainCategory { get; set; }
        public string Subcategory { get; set; }
        public string ProductLink { get; set; }
        public string Pagination { get; set; }
    }
}
