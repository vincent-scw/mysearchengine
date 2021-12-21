using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MySearchEngine.Server.Core;

namespace MySearchEngine.Server.BackgroundServices
{
    class StorageHostedService : BackgroundService
    {
        private readonly PageIndexer _pageIndexer;
        public StorageHostedService(PageIndexer pageIndexer)
        {
            _pageIndexer = pageIndexer;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(15 * 1000);

                // Store to disk every 15 seconds
                await _pageIndexer.StoreDataAsync();
            }
        }
    }
}
