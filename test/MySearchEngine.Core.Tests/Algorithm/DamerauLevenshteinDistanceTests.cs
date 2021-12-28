using MySearchEngine.Core.Algorithm;
using Xunit;

namespace MySearchEngine.Core.Tests.Algorithm
{
    public class DamerauLevenshteinDistanceTests
    {
        [Theory]
        [InlineData("CSharp", "", 6)]
        [InlineData("CSharp", "Shape", 3)]
        [InlineData("人工智能", "人工的智能", 1)]
        public void Should_AsExpected(string a, string b, int expectedDistance)
        {
            var distance = DamerauLevenshteinDistance.DistanceBetween(a, b, true);
            Assert.Equal(expectedDistance, distance);
        }
    }
}
