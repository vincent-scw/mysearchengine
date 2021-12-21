using Google.Protobuf.WellKnownTypes;
using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Utilities;
using Qctrl;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    class CrawlProcessingService : IProcessingService
    {
        private readonly IPageReader _pageReader;
        private readonly IIdGenerator<int> _idGenerator;
        private readonly QueueSvc.QueueSvcClient _queueClient;
        private readonly BooleanFilter _booleanFilter;

        public CrawlProcessingService(
            IPageReader pageReader,
            IIdGenerator<int> idGenerator,
            QueueSvc.QueueSvcClient queueClient)
        {
            _pageReader = pageReader;
            _idGenerator = idGenerator;
            _queueClient = queueClient;
            _booleanFilter = new BooleanFilter(100_000);
        }

        public void DoWork(string url, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(CrawlProcessingService)} started...");
            Task.Run(async () => await CrawlAsync(url, cancellationToken));
        }

        private async ValueTask CrawlAsync(string url, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var links = await HandleOneAsync(url);
            if(links == null)
                return;
            
            foreach (var link in links)
            {
                if (!_booleanFilter.TryAdd(link))
                    continue;

                await CrawlAsync(link, cancellationToken);
            }
        }

        private async Task<List<string>> HandleOneAsync(string url)
        {
            Console.Write($"Reading page at: {url}");
            var pi = await _pageReader.ReadAsync(new Uri(url));
            if (pi == null)
            {
                Console.WriteLine("  Ignored.");
                return null;
            }

            await _queueClient.EnqueueAsync(new Message()
            {
                Id = _idGenerator.Next(null),
                Url = url,
                Body = pi.Content,
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
            });

            Console.WriteLine("  Done.");
            return pi.Links;
        }
    }
}
