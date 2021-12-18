using System;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using MySearchEngine.Core;
using Qctrl;

namespace MySearchEngine.WebCrawler
{
    class CrawlProcessingService : IProcessingService
    {
        private readonly IPageReader _pageReader;
        private readonly IIdGenerator<int> _idGenerator;
        private readonly QueueSvc.QueueSvcClient _queueClient;
        private readonly ILogger<CrawlProcessingService> _logger;
        public CrawlProcessingService(
            IPageReader pageReader,
            IIdGenerator<int> idGenerator,
            QueueSvc.QueueSvcClient queueCient,
            ILogger<CrawlProcessingService> logger)
        {
            _pageReader = pageReader;
            _idGenerator = idGenerator;
            _queueClient = queueCient;
            _logger = logger;
        }

        public void DoWork(string uri, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(CrawlProcessingService)} started...");
            Task.Run(async () => await CrawlAsync(uri));
        }

        private async ValueTask CrawlAsync(string uri)
        {
            var pi = await _pageReader.ReadAsync(new Uri(uri));
            await _queueClient.EnqueueAsync(new Message()
            {
                Id = _idGenerator.Next(null),
                Body = pi.Content,
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
            });
        }
    }
}
