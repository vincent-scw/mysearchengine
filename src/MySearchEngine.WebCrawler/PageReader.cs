using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    internal class PageReader : IPageReader, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly CrawlerConfig _config;
        public PageReader(CrawlerConfig config)
        {
            _config = config;
            _httpClient = new HttpClient(
                new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip});
        }

        public async Task<PageInfo> ReadAsync(Uri uri)
        {
            var response = await _httpClient.GetAsync(uri);
            if (!_config.AllowedMediaTypes.Contains(response.Content.Headers.ContentType?.MediaType))
                return null;

            var htmlContent = await response.Content.ReadAsStringAsync();

            return new PageInfo(){Content = htmlContent, Links = ReadLinks(htmlContent)};
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private static List<string> ReadLinks(string content)
        {
            return null;
        }
    }
}
