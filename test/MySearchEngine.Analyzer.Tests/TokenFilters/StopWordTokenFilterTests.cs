using System;
using System.Collections.Generic;
using System.Text;
using MySearchEngine.Analyzer.TokenFilters;
using MySearchEngine.Analyzer.Tokenizers;
using Xunit;

namespace MySearchEngine.Analyzer.Tests.TokenFilters
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
