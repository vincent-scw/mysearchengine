using MySearchEngine.Analyzer.TokenFilters;
using MySearchEngine.Analyzer.Tokenizers;
using System.Collections.Generic;
using Xunit;

namespace MySearchEngine.Analyzer.Tests.TokenFilters
{
    public class StemmerTokenFilterTests
    {
        [Fact]
        public void Filter_ShouldAsExpected()
        {
            var tokenFilter = new StemmerTokenFilter();
            var tokens = tokenFilter.Filter(new List<Token>()
            {
                new Token(1, "ship") { Positions = new List<int>(){ 1 } },
                new Token(1, "ships") { Positions = new List<int>(){ 2 } },
            });

            Assert.Single(tokens);
            Assert.Contains(1, tokens[0].Positions);
            Assert.Contains(2, tokens[0].Positions);
        }
    }
}
