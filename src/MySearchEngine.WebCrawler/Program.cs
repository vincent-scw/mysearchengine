using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySearchEngine.Core.Utilities;
using Qctrl;
using System;
using System.Threading;
using Grpc.Net.Client;

namespace MySearchEngine.WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Arguments required.");
                return;
            }

            var host = CreateHostBuilder(args).Build();
            Console.WriteLine("Input url to start...");
            var url = Console.ReadLine();

            var cancellationToken = new CancellationTokenSource();
            var ps = host.Services.GetRequiredService<IProcessingService>();
            ps.DoWork(url, cancellationToken.Token);

            host.RunAsync();

            Console.ReadLine();
            Console.WriteLine("About to end...");
            cancellationToken.Cancel();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<BinRepository>();
                    services.AddSingleton<IProcessingService, CrawlProcessingService>();
                    services.AddSingleton<IPageReader, PageReader>();
                    services.AddSingleton<CrawlerConfig>();
                    services.AddSingleton(_ =>
                        new QueueSvc.QueueSvcClient(GrpcChannel.ForAddress(args[0],
                            new GrpcChannelOptions() { Credentials = ChannelCredentials.Insecure })));
                    services.AddSingleton<IIdGenerator<int>, IntegerIdGenerator>();
                });
        }
    }
}
