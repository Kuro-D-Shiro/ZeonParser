using System.Net.Http;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ZeonDownloadAndSaveImageService(IImageDownloader imageDownloader,
        IImageSaver imageSaver) : IDownloadAndSaveImageService
    {
        private readonly IImageDownloader imageDownloader = imageDownloader;
        private readonly IImageSaver imageSaver = imageSaver;

        public async Task<string?> DownloadAndSaveImage(string url, Guid guid)
        {
            string cleanPath = url.Split('?', '#')[0];
            var imageFormat = Path.GetExtension(cleanPath) ?? "";
            if (imageFormat == "")
                return null;

            byte[] imageBytes = await imageDownloader.Download(url);
            var imageFileName = $"{guid}{imageFormat}";
            var result = await imageSaver.Save(imageBytes, $"ProductImages/{imageFileName}");
            return imageFileName;
        }
    }
}
