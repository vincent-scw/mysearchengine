using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.TokenFilters;
using Xunit;

namespace MySearchEngine.Core.Tests.Analyzer.TokenFilters
{
    public class LowercaseTokenFilterTests
    {
        [Fact]
        public void Should_FormatToLowercase()
        {
            var tokenFilter = new LowercaseTokenFilter();
            var tokenList = new List<Token>
            {
                new Token(1, "Some"), 
                new Token(2, "ANY"), 
                new Token(3, "A_B"), 
                new Token(4, "StringExt")
            };
            tokenList = tokenFilter.Filter(tokenList);

            Assert.Equal("some|any|a_b|stringext", string.Join('|', tokenList.Select(x => x.Term).ToArray()));
        }
    }
}
