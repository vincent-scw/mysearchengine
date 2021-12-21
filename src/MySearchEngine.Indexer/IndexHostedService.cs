using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Hosting;
using Qctrl;

namespace MySearchEngine.Indexer
{
    class IndexHostedService : BackgroundService
    {
        private readonly QueueSvc.QueueSvcClient _queueClient;

        public IndexHostedService(QueueSvc.QueueSvcClient queueClient)
        {
            _queueClient = queueClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = await _queueClient.ReadAsync(new Empty());
                if (message == null) continue;

                Console.WriteLine($"Handling message {message.Id}...");

                Thread.Sleep(100);
            }
        }
    }
}
