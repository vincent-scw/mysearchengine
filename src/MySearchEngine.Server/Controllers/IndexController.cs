using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySearchEngine.Core;

namespace MySearchEngine.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase, IDisposable
    {
        private readonly IDocIndexer _docIndexer;
        private readonly ILogger<IndexController> _logger;
        private HttpClient _httpClient;

        public IndexController(
            IDocIndexer docIndexer,
            ILogger<IndexController> logger)
        {
            _docIndexer = docIndexer;
            _logger = logger;
            _httpClient = new HttpClient(
                new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
        }

        [HttpPost("force")]
        public async Task<ActionResult> ForceIndex(string url, int docId)
        {
            var response = await _httpClient.GetAsync(url);
            var htmlContent = await response.Content.ReadAsStringAsync();
            var title = ReadTitle(htmlContent);

            var docInfo = new DocInfo(docId, title, url);
            _docIndexer.Index(docInfo, htmlContent);

            return Ok();
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private string ReadTitle(string content)
        {
            var t = content.IndexOf("<title>", StringComparison.Ordinal);
            if (t < 0)
            {
                return string.Empty;
            }

            var tStart = t + 7;
            var tEnd = content.IndexOf("</title>", StringComparison.Ordinal) - 1;
            var title = content[tStart..tEnd];
            return title.Split('|')[0].Trim();
        }
    }
}
