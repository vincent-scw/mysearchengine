using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    internal class PageReader : IPageReader, IDisposable
    {
        private const string LinkPattern = "(http|ftp|https):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])";

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

        private List<string> ReadLinks(string content)
        {
            var regex = new Regex(LinkPattern);
            var matches = regex.Matches(content).Select(x => x.Value);

            return matches.Where(match => match.StartsWith(_config.CrawlLinkPrefix)
                                          && !_config.ExcludeLinkSuffix.Any(match.EndsWith)).ToList();
        }
    }
}
