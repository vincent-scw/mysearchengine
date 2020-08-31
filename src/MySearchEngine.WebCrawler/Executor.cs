using MySearchEngine.WebCrawler.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MySearchEngine.WebCrawler
{
    class Executor
    {
        private readonly BufferBlock<string> _bufferBlock;
        private readonly TransformBlock<string, string> _downloadBlock;

        public Executor()
        {
            _bufferBlock = new BufferBlock<string>();
            _downloadBlock = new TransformBlock<string, string>(async (uri) =>
            {
                Console.WriteLine("Downloading '{0}'", uri);

                try
                {
                    var content = await new HttpClient(
                        new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
                    .GetStringAsync(uri);

                    var htmlInfo = HtmlExtractor.Extract(content);
                    htmlInfo.Links.ForEach(l => _bufferBlock.SendAsync(l));

                    return htmlInfo.Content;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine("Ignore '{0}': {1}", uri, ex.Message);
                    return null;
                }
            });

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            _bufferBlock.LinkTo(_downloadBlock, linkOptions);
        }

        public async Task StartAsync(string uri, CancellationToken cancellationToken)
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
