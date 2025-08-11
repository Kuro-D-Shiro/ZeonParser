namespace ZeonService.Parser.Interfaces
{
    public interface IDownloadAndSaveImageService
    {
        Task<string?> DownloadAndSaveImage(string url, string path);
    }
}
