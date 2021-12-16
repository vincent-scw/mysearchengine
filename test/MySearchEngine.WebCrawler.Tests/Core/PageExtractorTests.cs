using MySearchEngine.WebCrawler.Core;
using System.Linq;
using Xunit;

namespace MySearchEngine.WebCrawler.Tests.Core
{
    public class PageExtractorTests
    {
        [Fact]
        public void ExtractLinks_Should_Succeed()
        {
            var extractor = new PageExtractor();
            var pageInfo = extractor.Extract("<a href=\"https:\\\\website.com\">somewhere</a>");

            Assert.Equal(1, pageInfo.links.Count());
            Assert.Equal("https:\\\\website.com", pageInfo.links.First());
        }

        [Fact]
        public void ExtractContent_Should_AsExpected()
        {
            var extractor = new PageExtractor();
            var pageInfo = extractor.Extract(@"
<html>
    <head>
        <style></style>
    </head>
    <body>
        <div>Something</div>
        <script>var abc = function()</script>
    </body>
</html>
");

            Assert.Equal("Something", pageInfo.content.Trim());
        }
    }
}
