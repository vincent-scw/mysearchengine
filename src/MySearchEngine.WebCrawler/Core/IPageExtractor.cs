using System.Collections.Generic;

namespace MySearchEngine.WebCrawler.Core
{
    internal interface IPageExtractor
    {
        (IEnumerable<string> links, string content) Extract(string htmlContent);
    }
}
