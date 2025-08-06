using Microsoft.Extensions.Options;
using ZeonService.Parser.Interfaces;
using ZeonService.Parser.Settings;

namespace ZeonService.Parser
{
    public class HtmlLoader(IOptions<ZeonParserSettings> options, IHttpClientFactory httpClientFactory) : IHtmlLoader
    {
        private readonly ZeonParserSettings _parserSettings = options.Value;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<string> LoadPageByURL(string url)
        {
            string html = "";
            using (var httpClient = _httpClientFactory.CreateClient(url))
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", _parserSettings.UserAgent);
                var response = await httpClient.GetAsync(url);
                html = response != null && response.IsSuccessStatusCode ?
                    await response.Content.ReadAsStringAsync()
                    : "";
            }
            return html;
        }
    }
}
