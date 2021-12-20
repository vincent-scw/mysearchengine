using System.Collections.Generic;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.TokenFilters;
using Xunit;

namespace MySearchEngine.Core.Tests.Analyzer.TokenFilters
{
    public class StopWordTokenFilterTests
    {
        [Fact]
        public void Should_RemoveStopWord()
        {
            var stopWords = new List<string> {"the"};
            var tokenFilter = new StopWordTokenFilter(stopWords);
            var tokens = new List<Token>()
            {
                new Token(1, "the"),
                new Token(2, "hey")
            };
            tokens = tokenFilter.Filter(tokens);

            Assert.Single(tokens);
            Assert.Equal("hey", tokens[0].Term);
        }
    }
}
