using System.Collections.Generic;

namespace MySearchEngine.WebCrawler
{
    class PageInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Links { get; set; }
    }
}
