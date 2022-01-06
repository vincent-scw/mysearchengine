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
                new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip})
            {
                Timeout = TimeSpan.FromMilliseconds(5000)
            };
        }

        public async Task<PageInfo> ReadAsync(Uri uri)
        {
            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (!_config.AllowedMediaTypes.Contains(response.Content.Headers.ContentType?.MediaType))
                    return null;

                var htmlContent = await response.Content.ReadAsStringAsync();
                var title = ReadTitle(htmlContent);
                // No title
                if (string.IsNullOrWhiteSpace(title))
                    return null;

                return new PageInfo()
                {
                    Title = title,
                    Content = htmlContent,
                    Links = ReadLinks(uri, htmlContent)
                };
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private string ReadTitle(string content)
        {
            var t = content.IndexOf("<title>", StringComparison.Ordinal);
            if (t < 0)
            {
                return string.Empty;
            }

            var tStart = t + 7;
            var tEnd = content.IndexOf("</title>", StringComparison.Ordinal) - 1;
            var title = content[tStart..tEnd];
            return title.Split('|')[0].Trim();
        }

        private List<string> ReadLinks(Uri uri, string content)
        {
            var regex = new Regex(LinkPattern);
            var matches = regex.Matches(content).Select(x => x.Value);

            return matches.Where(match => match.StartsWith($"{uri.Scheme}://{uri.Host}")
                                          && !_config.ExcludeLinkSuffix.Any(match.EndsWith)).ToList();
        }
    }
}
