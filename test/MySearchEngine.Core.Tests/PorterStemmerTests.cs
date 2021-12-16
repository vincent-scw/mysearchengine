using Xunit;

namespace MySearchEngine.Core.Tests
{
    public class PorterStemmerTests
    {
        [Theory]
        [InlineData("ships", "ship")]
        //[InlineData("tries", "try")]
        //[InlineData("matting", "mate")]
        public void Stemmer_ShouldAsExpected(string input, string output)
        {
            var stemmer = new PorterStemmer();
            var result = stemmer.StemWord(input);
            Assert.Equal(output, result);
        }
    }
}
