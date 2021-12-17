using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MySearchEngine.QueueService
{
    sealed class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundQueue _queue;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(
            IBackgroundQueue queue,
            ILogger<QueuedHostedService> logger) =>
            (_queue, _logger) = (queue, logger);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(QueuedHostedService)} is running.");
            return ProcessQueueAsync(stoppingToken);
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {

            }
        }
    }
}
