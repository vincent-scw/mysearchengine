using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.WebCrawler
{
    public class CrawlerConfig
    {
        public IEnumerable<string> AllowedMediaTypes => new List<string> { "text/html" };
    }
}
