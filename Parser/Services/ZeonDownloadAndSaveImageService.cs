using FluentResults;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ZeonDownloadAndSaveImageService(IImageDownloader imageDownloader,
        IImageSaver imageSaver) : IDownloadAndSaveImageService
    {
        private readonly IImageDownloader imageDownloader = imageDownloader;
        private readonly IImageSaver imageSaver = imageSaver;
        
        public async Task<Result<string>> DownloadAndSaveImage(string url, Guid guid)
        {
            string cleanPath = url.Split('?', '#')[0];
            var imageFormat = Path.GetExtension(cleanPath) ?? "";
            if (imageFormat == "")
                return Result.Fail(new Error("Полученный url не является ссылкой на изображение."));

            var imageBytes = await imageDownloader.Download(url);
            var imageFileName = $"{guid}{imageFormat}";

            var imageSaveResult = await imageSaver.Save(imageBytes, $"ProductImages/{imageFileName}");
            if (imageSaveResult.IsFailed)
                return imageSaveResult;

            return imageFileName;
        }
    }
}
