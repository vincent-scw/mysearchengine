using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    public class WebCrawler
    {
        public async Task Crawl(Uri uri, CancellationToken cancellationToken)
        {
            var result = await new HttpClient().GetAsync(uri.AbsoluteUri);
            var content = await result.Content.ReadAsStringAsync();

            var extractor = new HtmlExtractor();
            extractor.Extract(content);
        }
    }
}
