using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Qctrl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.Server.Indexer
{
    class IndexHostedService : BackgroundService
    {
        private readonly QueueSvc.QueueSvcClient _queueClient;
        private readonly PageIndexer _pageIndexer;

        public IndexHostedService(
            QueueSvc.QueueSvcClient queueClient,
            PageIndexer pageIndexer)
        {
            _queueClient = queueClient;
            _pageIndexer = pageIndexer;
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

                Console.WriteLine($"Handling message {message.Id}...");

                await _pageIndexer.IndexAsync(message);
            }
        }
    }
}
