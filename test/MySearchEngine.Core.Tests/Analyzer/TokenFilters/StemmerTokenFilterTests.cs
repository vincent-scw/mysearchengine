using System.Collections.Generic;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.TokenFilters;
using Xunit;

namespace MySearchEngine.Core.Tests.Analyzer.TokenFilters
{
    public class StemmerTokenFilterTests
    {
        [Fact]
        public void Filter_ShouldAsExpected()
        {
            var tokenFilter = new StemmerTokenFilter();
            var tokens = tokenFilter.Filter(new List<Token>()
            {
                new Token("ship") { Positions = new List<int>(){ 1 } },
                new Token("ships") { Positions = new List<int>(){ 2 } },
            });

            Assert.Single(tokens);
            Assert.Contains(1, tokens[0].Positions);
            Assert.Contains(2, tokens[0].Positions);
        }
    }
}
