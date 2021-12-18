using System;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.WebCrawler
{
    class CrawlProcessingService : IProcessingService
    {
        private readonly IPageReader _pageReader;
        private readonly ILogger<CrawlProcessingService> _logger;
        public CrawlProcessingService(
            IPageReader pageReader,
            ILogger<CrawlProcessingService> logger)
        {
            _pageReader = pageReader;
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
        }
    }
}
