namespace ZeonService.Parser.Interfaces
{
    public interface IHtmlLoader
    {
        Task<string> LoadPageByURL(string url);
    }
}
