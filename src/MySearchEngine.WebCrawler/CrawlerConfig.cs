using System.Collections.Generic;

namespace MySearchEngine.WebCrawler
{
    public class CrawlerConfig
    {
        public List<string> AllowedMediaTypes => new List<string> { "text/html" };
        public List<string> ExcludeLinkSuffix => new List<string> {".css", ".js", "jpg"};
        public string CrawlLinkPrefix => "https://www.differencebetween.com/";
    }
}
