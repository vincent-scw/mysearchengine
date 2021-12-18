using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySearchEngine.WebCrawler.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            Console.WriteLine("Input url to start...");
            var url = Console.ReadLine();

            var cancellationToken = new CancellationTokenSource();
            var ps = host.Services.GetRequiredService<IProcessingService>();
            ps.DoWork(url, cancellationToken.Token);

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IProcessingService, CrawlProcessingService>();
                    
                    services.AddSingleton<IPageReader, PageReader>();
                    services.AddSingleton<IPageExtractor, PageExtractor>();
                    services.AddSingleton<Executor>();
                    services.AddSingleton<CrawlerConfig>();
                });
        }
    }
}
