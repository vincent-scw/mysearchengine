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
        private readonly PageIndexer _pageIndexer;
        private readonly ILogger<IndexHostedService> _logger;

        public IndexHostedService(
            QueueSvc.QueueSvcClient queueClient,
            PageIndexer pageIndexer,
            ILogger<IndexHostedService> logger)
        {
            _queueClient = queueClient;
            _pageIndexer = pageIndexer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueClient.ReadAsync(new Empty());
                if (message == null)
                {
                    Thread.Sleep(300);
                    continue;
                }

                _logger.LogInformation($"Handling message {message.Id}...");

                try
                {
                    var pageInfo = new PageInfo() {Id = message.Id, Url = message.Url, Content = message.Body};
                    _pageIndexer.Index(pageInfo);

                    //await _queueClient.AckAsync(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Index Error");
                }
            }
        }
    }
}
