using FluentResults;

namespace ZeonService.Parser.Interfaces
{
    public interface IDownloadAndSaveImageService
    {
        Task<Result<string>> DownloadAndSaveImage(string url, Guid guid);
    }
}
