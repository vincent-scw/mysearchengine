using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySearchEngine.Server.Core;

namespace MySearchEngine.Server.BackgroundServices
{
    class StorageHostedService : BackgroundService
    {
        private readonly IDocIndexer _docIndexer;
        private readonly ILogger<StorageHostedService> _logger;
        public StorageHostedService(
            IDocIndexer docIndexer,
            ILogger<StorageHostedService> logger)
        {
            _docIndexer = docIndexer;
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
                await Task.Delay(15 * 1000, stoppingToken);

                try
                {
                    // Store to disk every 15 seconds
                    await _docIndexer.StoreDataAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Store data error");
                }
            }
        }
    }
}
