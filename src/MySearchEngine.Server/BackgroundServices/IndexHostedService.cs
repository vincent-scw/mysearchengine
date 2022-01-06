using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySearchEngine.Server.Core;
using Qctrl;

namespace MySearchEngine.Server.BackgroundServices
{
    class IndexHostedService : BackgroundService
    {
        private readonly QueueSvc.QueueSvcClient _queueClient;
        private readonly IDocIndexer _docIndexer;
        private readonly ILogger<IndexHostedService> _logger;

        public IndexHostedService(
            QueueSvc.QueueSvcClient queueClient,
            IDocIndexer docIndexer,
            ILogger<IndexHostedService> logger)
        {
            _queueClient = queueClient;
            _docIndexer = docIndexer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(IndexHostedService)} is running.");
            return BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueClient.ReadAsync(new Empty());
                if (message.Id <= 0)
                {
                    await Task.Delay(300, stoppingToken);
                    continue;
                }

                _logger.LogInformation($"Handling message {message.Id}...");

                try
                {
                    // Don't need to store page content
                    var docInfo = new DocInfo(-1, message.Title, message.Url);
                    _docIndexer.Index(docInfo, message.Body);

                    await _queueClient.AckAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Index error");
                }
            }
        }
    }
}
