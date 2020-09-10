using MySearchEngine.WebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler.Core
{
    interface IPageDownloader
    {
        Task<PageInfo> DownloadAsync(Uri uri);
    }
}
