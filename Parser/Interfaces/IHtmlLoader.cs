namespace ZeonParser.Parser.Interfaces
{
    public interface IHtmlLoader
    {
        public Task<string> LoadPageByURL(string url);
    }
}
