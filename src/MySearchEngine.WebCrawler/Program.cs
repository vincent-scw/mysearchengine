using System;
using System.Threading;
using System.Threading.Tasks;
using MySearchEngine.WebCrawler.Core;

namespace MySearchEngine.WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start crawling...press any key to cancel...");
            var cancellationToken = new CancellationTokenSource();

            var executor = new Executor(null);
            await executor.StartAsync(new Uri("https://www.differencebetween.com/"), cancellationToken.Token);

            Console.ReadLine();
            cancellationToken.Cancel();

            Console.WriteLine("Processing cancelled.");
            Console.ReadLine();
        }
    }
}
