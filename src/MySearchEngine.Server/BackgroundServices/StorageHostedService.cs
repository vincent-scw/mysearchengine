using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySearchEngine.Server.Core;

namespace MySearchEngine.Server.BackgroundServices
{
    class StorageHostedService : BackgroundService
    {
        private readonly PageIndexer _pageIndexer;
        private readonly ILogger<StorageHostedService> _logger;
        public StorageHostedService(
            PageIndexer pageIndexer,
            ILogger<StorageHostedService> logger)
        {
            _pageIndexer = pageIndexer;
            _logger = logger;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(StorageHostedService)} is running.");
            return BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
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
