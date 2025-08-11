using Microsoft.Extensions.Options;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser.Services
{
    public class HtmlLoader(IOptions<ZeonParserSettings> options, IHttpClientFactory httpClientFactory) : IHtmlLoader
    {
        private readonly ZeonParserSettings parserSettings = options.Value;
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        public async Task<string> Download(string url)
        {
            string html = "";
            using (var httpClient = httpClientFactory.CreateClient(url))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", parserSettings.UserAgent);
                var response = await httpClient.GetAsync(url);
                html = response != null && response.IsSuccessStatusCode ?
                    await response.Content.ReadAsStringAsync()
                    : "";
            }
            return html;
        }

/*      public async Task<string> LoadPageByURL(string url)
        {
            string html = "";
            using (var httpClient = httpClientFactory.CreateClient(url))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", parserSettings.UserAgent);
                var response = await httpClient.GetAsync(url);
                html = response != null && response.IsSuccessStatusCode ?
                    await response.Content.ReadAsStringAsync()
                    : "";
            }
            return html;
        }*/
    }
}
