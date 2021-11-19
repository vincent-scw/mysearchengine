using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySearchEngine.WebCrawler.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySearchEngine.WebCrawler.Tests.Core
{
    [TestClass]
    public class PageExtractorTests
    {
        [TestMethod]
        public void ExtractLinks_Should_Succeed()
        {
            var extractor = new PageExtractor();
            var pageInfo = extractor.Extract("<a href=\"https:\\\\website.com\">somewhere</a>");

            Assert.AreEqual(1, pageInfo.links.Count());
            Assert.AreEqual("https:\\\\website.com", pageInfo.links.First());
        }

        [TestMethod]
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

            Assert.AreEqual("Something", pageInfo.content.Trim());
        }
    }
}
