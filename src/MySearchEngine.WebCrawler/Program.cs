using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MySearchEngine.Core;
using MySearchEngine.Core.Utilities;
using Qctrl;

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
                    services.AddSingleton<CrawlerConfig>();
                    services.AddSingleton((sp) =>
                        new QueueSvc.QueueSvcClient(new Channel("localhost", 10024, ChannelCredentials.Insecure)));
                    services.AddSingleton<IIdGenerator<int>, IntegerIdGenerator>();
                });
        }
    }
}
