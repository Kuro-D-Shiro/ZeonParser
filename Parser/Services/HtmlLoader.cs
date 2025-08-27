using ZeonService.Parser.Interfaces;

namespace ZeonService.Parser.Services
{
    public class HtmlLoader(IHttpClientFactory httpClientFactory) : IHtmlLoader
    {
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

        public async Task<string> Download(string url)
        {
            string html = "";
            using (var httpClient = httpClientFactory.CreateClient(url))
            {
                var response = await httpClient.GetAsync(url);
                html = response != null && response.IsSuccessStatusCode ?
                    await response.Content.ReadAsStringAsync()
                    : "";
            }
            return html;
        }
    }
}
