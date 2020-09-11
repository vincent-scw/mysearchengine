using MySearchEngine.Core;
using MySearchEngine.Repository;
using MySearchEngine.WebCrawler.Core;
using MySearchEngine.WebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly TransformBlock<Uri, PageInfo> _downloadBlock;
        private readonly TransformBlock<PageInfo, string> _indexBlock;

        private readonly IPageDownloader _pageDownloader;
        private readonly ICrawledRepository _crawledRepository;
        private readonly IIndexRepository _indexRepository;

        private readonly IIdGenerator<int> _termIdGenerator;

        public Executor(
            IPageDownloader downloader,
            ICrawledRepository crawledRepository,
            IIndexRepository indexRepository)
        {
            _pageDownloader = downloader ?? throw new ArgumentNullException(nameof(IPageDownloader));
            _crawledRepository = crawledRepository ?? throw new ArgumentNullException(nameof(ICrawledRepository));
            _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(IIndexRepository));

            _termIdGenerator = new IntegerIdGenerator();

            // Build data flow
            _bufferBlock = new BufferBlock<Uri>();
            _downloadBlock = new TransformBlock<Uri, PageInfo>(BuildDownloadTask());
            _indexBlock = new TransformBlock<PageInfo, string>(BuildIndexTask());

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            _bufferBlock.LinkTo(_downloadBlock, linkOptions);
            _downloadBlock.LinkTo(_indexBlock, linkOptions);
        }

        private Func<Uri, Task<PageInfo>> BuildDownloadTask()
        {
            return async (uri) =>
            {
                Console.WriteLine("Downloading '{0}'", uri);

                try
                {
                    var htmlInfo = await _pageDownloader.DownloadAsync(uri);
                    htmlInfo.Links.ToList().ForEach(newUri => {
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

        private Func<PageInfo, string> BuildIndexTask()
        {
            return (pageInfo) =>
            {
                return "";
            };
        }

        public async Task StartAsync(Uri uri, CancellationToken cancellationToken)
        {
            _crawledRepository.AddIfNew(uri);
            await _bufferBlock.SendAsync(uri);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(500);
            }

            _bufferBlock.Complete();
        }
    }
}
