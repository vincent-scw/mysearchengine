using Google.Protobuf.WellKnownTypes;
using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Utilities;
using Qctrl;
using System;
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
        private readonly BinRepository _binRepository;

        public CrawlProcessingService(
            IPageReader pageReader,
            IIdGenerator<int> idGenerator,
            BinRepository binRepository,
            QueueSvc.QueueSvcClient queueClient)
        {
            _pageReader = pageReader;
            _idGenerator = idGenerator;
            _queueClient = queueClient;
            _binRepository = binRepository;
            _booleanFilter = new BooleanFilter(binRepository.ReadBooleanFilterAsync(100_000).Result);
        }

        public void DoWork(string url, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{nameof(CrawlProcessingService)} started...");
            Task.Run(async () => await CrawlAsync(url, cancellationToken));
            Task.Run(async () => await StoreProcessing(cancellationToken));
        }

        private async ValueTask CrawlAsync(string url, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var pi = await HandleOneAsync(url);
            if(pi?.Links == null)
                return;

            foreach (var link in pi.Links)
            {
                if (!_booleanFilter.TryAdd(link))
                    continue;

                await CrawlAsync(link, cancellationToken);
            }
        }

        private async Task<PageInfo> HandleOneAsync(string url)
        {
            Console.Write($"Reading page at: {url}");
            var pi = await _pageReader.ReadAsync(new Uri(url));
            // Add title to boolean filter as well, to avoid different links target to same page
            if (pi == null || !_booleanFilter.TryAdd(pi.Title))
            {
                Console.WriteLine("  Ignored.");
                return null;
            }
            
            await _queueClient.EnqueueAsync(new Message()
            {
                Id = _idGenerator.Next(null),
                Title = pi.Title,
                Url = url,
                Body = pi.Content,
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.Now)
            });

            Console.WriteLine("  Done.");
            return pi;
        }

        private async Task StoreProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(15 * 1000, stoppingToken);

                try
                {
                    // Store to disk every 15 seconds
                    await _binRepository.StoreBooleanFilterAsync(_booleanFilter.BooleanArray);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
