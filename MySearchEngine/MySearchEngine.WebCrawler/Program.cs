using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var crawler = new WebCrawler();
            await crawler.Crawl(new Uri("https://www.differencebetween.com/"), CancellationToken.None);
        }
    }
}
