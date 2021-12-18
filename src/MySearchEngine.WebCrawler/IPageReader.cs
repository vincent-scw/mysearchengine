using System;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    interface IPageReader
    {
        Task<PageInfo> ReadAsync(Uri uri);
    }
}
