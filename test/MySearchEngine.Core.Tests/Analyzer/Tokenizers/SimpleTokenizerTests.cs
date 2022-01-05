using MySearchEngine.Core.Analyzer.Tokenizers;
using System.Linq;
using Xunit;

namespace MySearchEngine.Core.Tests.Analyzer.Tokenizers
{
    public class SimpleTokenizerTests
    {
        [Theory]
        [InlineData("  The QUICK brown foxes   jumped  over the dog!")]
        [InlineData("  The QUICK brown foxes jumped  over the dog!  ")]
        public void Tokenize_ShouldAsExpected(string text)
        {
            var tokenizer = new SimpleTokenizer();
            var tokens = tokenizer.Tokenize(text);
            Assert.Equal(8, tokens.Count());
        }
    }
}
