using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MySearchEngine.Repository;
using MySearchEngine.WebCrawler.Core;

namespace MySearchEngine.WebCrawler
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start crawling...press any key to cancel...");
            RegisterServices();

            var cancellationToken = new CancellationTokenSource();

            var executor = _serviceProvider.GetService<Executor>();
            await executor.StartAsync(new Uri("https://www.differencebetween.com/"), cancellationToken.Token);

            Console.ReadLine();
            cancellationToken.Cancel();

            Console.WriteLine("Processing cancelled.");

            DisposeServices();
            Console.ReadLine();
        }

        private static void RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IPageDownloader, PageDownloader>();
            services.AddSingleton<IPageExtractor, PageExtractor>();
            services.AddSingleton<ICrawledRepository, DictionaryCrawledRepository>();
            services.AddSingleton<IIndexRepository, IndexRepository>();
            services.AddSingleton<Executor>();
            services.AddSingleton<CrawlerConfig>();

            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
