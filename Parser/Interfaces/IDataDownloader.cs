namespace ZeonService.Parser.Interfaces
{
    public interface IDataDownloader<T>
    {
        Task<T> Download(string url);   
    }
}
