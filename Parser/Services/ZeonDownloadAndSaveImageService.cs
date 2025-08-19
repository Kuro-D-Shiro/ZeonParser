using System.Net.Http;
using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class ZeonDownloadAndSaveImageService(IImageDownloader imageDownloader,
        IImageSaver imageSaver) : IDownloadAndSaveImageService
    {
        private readonly IImageDownloader imageDownloader = imageDownloader;
        private readonly IImageSaver imageSaver = imageSaver;

        public async Task<string?> DownloadAndSaveImage(string url, string productId)
        {
            string cleanPath = url.Split('?', '#')[0];
            var imageFormat = Path.GetExtension(cleanPath) ?? "";
            if (imageFormat == "")
                return null;

            byte[] imageBytes = await imageDownloader.Download(url);
            var imageFileName = $"{SanitizeFileName(productId)}{imageFormat}";
            await imageSaver.Save(imageBytes, $"ProductImages/{imageFileName}");
            return imageFileName;
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
