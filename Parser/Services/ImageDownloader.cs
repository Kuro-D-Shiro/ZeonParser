using Microsoft.Extensions.Options;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Services
{
    public class ImageDownloader(IOptions<ZeonParserSettings> options, IHttpClientFactory httpClientFactory) : IImageDownloader
    {
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        public async Task<byte[]> Download(string url)
        {
            using (var httpClient = httpClientFactory.CreateClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", parserSettings.UserAgent);
                return await httpClient.GetByteArrayAsync(url);
            }
        }
    }
}
