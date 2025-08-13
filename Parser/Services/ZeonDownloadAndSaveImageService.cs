using System.Net.Http;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ZeonDownloadAndSaveImageService(IImageDownloader imageDownloader,
        IImageSaver imageSaver) : IDownloadAndSaveImageService
    {
        private readonly IImageDownloader imageDownloader = imageDownloader;
        private readonly IImageSaver imageSaver = imageSaver;

        public async Task<string?> DownloadAndSaveImage(string url, string productName)
        {
            string cleanPath = url.Split('?', '#')[0];
            var imageFormat = Path.GetExtension(cleanPath) ?? "";
            if (imageFormat == "")
                return null;

            byte[] imageBytes = await imageDownloader.Download(url);
            var imagePath = $"ProductImages/{SanitizeFileName(productName)}{imageFormat}";
            await imageSaver.Save(imageBytes, imagePath);
            return imagePath;
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }
            return fileName;
        }
    }
}
