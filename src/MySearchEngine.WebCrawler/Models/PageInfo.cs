using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.WebCrawler.Models
{
    public class PageInfo
    {
        public PageInfo(Uri uri)
        {
            Uri = uri;
            Links = new List<string>();
            Analyzable = true;
        }

        public Uri Uri { get; }
        public IEnumerable<string> Links { get; set; }
        public string OriginContent { get; set; }
        public string PurifiedContent { get; set; }
        public bool Analyzable { get; set; }
    }
}
