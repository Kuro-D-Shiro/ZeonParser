namespace ZeonService.Parser.Interfaces
{
    public interface IDownloadAndSaveImageService
    {
        Task<string?> DownloadAndSaveImage(string url, Guid guid);
    }
}
