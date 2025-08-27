using FluentResults;
using Microsoft.Extensions.Options;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Services
{
    public class ImageDownloader(IHttpClientFactory httpClientFactory,
        ILogger<ImageDownloader> logger) : IImageDownloader
    {
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;
        private readonly ILogger<ImageDownloader> logger = logger;

        public async Task<byte[]> Download(string url)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            {
                var imageBytes = await httpClient.GetByteArrayAsync(url);
                if (!imageBytes.Any())
                    logger.LogError("Не удалось загрузить картинку по url: {url}", url);
                return imageBytes;
            }
        }
    }
}
