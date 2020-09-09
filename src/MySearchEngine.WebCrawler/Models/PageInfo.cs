using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.WebCrawler.Models
{
    public class PageInfo
    {
        public Uri Uri { get; set; }
        public List<string> Links { get; set; }
        public string Content { get; set; }
    }
}
