using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySearchEngine.Server.Core;

namespace MySearchEngine.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    class SearchEngineController : ControllerBase
    {
        private readonly SearchEngine _searchEngine;
        private readonly ILogger<SearchEngineController> _logger;

        public SearchEngineController(
            SearchEngine searchEngine,
            ILogger<SearchEngineController> logger)
        {
            _searchEngine = searchEngine;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Search([FromQuery] string searchText, [FromQuery] int size = 10, [FromQuery] int from = 0)
        {
            _logger.LogInformation($"Search for \"{searchText}\"");
            var searchResult = _searchEngine.Search(searchText, size, from);
            return Ok(searchResult);
        }
    }
}
