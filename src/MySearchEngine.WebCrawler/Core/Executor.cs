using MySearchEngine.Repository;
using MySearchEngine.WebCrawler.Core;
using MySearchEngine.WebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MySearchEngine.WebCrawler.Core
{
    class Executor
    {
        private readonly BufferBlock<Uri> _bufferBlock;
        private readonly TransformBlock<Uri, HtmlInfo> _downloadBlock;

        private readonly ICrawledRepository _crawledRepository;

        public Executor(
            ICrawledRepository crawledRepository)
        {
            _crawledRepository = crawledRepository ?? new DictionaryCrawledRepository();

            _bufferBlock = new BufferBlock<Uri>();
            _downloadBlock = new TransformBlock<Uri, HtmlInfo>(DownloadTask());

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            _bufferBlock.LinkTo(_downloadBlock, linkOptions);
        }

        private Func<Uri, Task<HtmlInfo>> DownloadTask()
        {
            return async (uri) =>
            {
                Console.WriteLine("Downloading '{0}'", uri);

                try
                {
                    var content = await new HttpClient(
                        new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
                    .GetStringAsync(uri);

                    var htmlInfo = HtmlExtractor.Extract(content);
                    htmlInfo.Links.ForEach(l => {
                        var newUri = new Uri(l);
                        // Pre-add uri to repository
                        if (_crawledRepository.AddIfNew(newUri))
                        {
                            _bufferBlock.SendAsync(newUri);
                        }
                    });

                    return htmlInfo;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Ignore '{0}': {1}", uri, ex.Message);
                    return null;
                }
            };
        }

        public async Task StartAsync(Uri uri, CancellationToken cancellationToken)
        {
            await _bufferBlock.SendAsync(uri);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }

            _bufferBlock.Complete();
        }
    }
}
