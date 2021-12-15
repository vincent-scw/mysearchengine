using MySearchEngine.Analyzer.CharacterFilters;
using Xunit;

namespace MySearchEngine.Analyzer.Tests.CharacterFilters
{
    public class HtmlElementFilterTests
    {
        [Fact]
        public void HtmlElement_ShouldBeFiltered()
        {
            var htmlText = @"
<html>
    <head>
        <style></style>
    </head>
    <body>
        <div>Something</div>
        <script>var abc = function()</script>
    </body>
</html>
";

            var filter = new HtmlElementFilter();
            var result = filter.Filter(htmlText);
            Assert.Equal(htmlText.Length, result.Length);
            Assert.Equal("Something", result.Trim());
        }
    }
}
