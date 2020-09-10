using MySearchEngine.WebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler.Core
{
    internal class PageDownloader : IPageDownloader
    {
        private readonly IPageExtractor _pageExtractor;
        private readonly CrawlerConfig _config;
        public PageDownloader(IPageExtractor pageExtractor, CrawlerConfig config)
        {
            _pageExtractor = pageExtractor ?? throw new ArgumentNullException(nameof(IPageExtractor));
            _config = config;
        }

        public async Task<PageInfo> DownloadAsync(Uri uri)
        {
            using (var client = new HttpClient(
                       new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }))
            {
                var response = await client.GetAsync(uri);

                if (!_config.AllowedMediaTypes.Contains(response.Content.Headers.ContentType?.MediaType))
                {
                    return new PageInfo(uri) { Analyzable = false };
                }

                var htmlContent = await response.Content.ReadAsStringAsync();

                var (links, content) = _pageExtractor.Extract(htmlContent);
                return new PageInfo(uri)
                {
                    Links = links.Where(x => Uri.IsWellFormedUriString(x, UriKind.Absolute)).Select(l => new Uri(l)),
                    OriginContent = htmlContent,
                    PurifiedContent = content,
                    Analyzable = true
                };
            }
        }
    }
}
