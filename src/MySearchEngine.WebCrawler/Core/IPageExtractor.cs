using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.WebCrawler.Core
{
    internal interface IPageExtractor
    {
        (IEnumerable<string> links, string content) Extract(string htmlContent);
    }
}
