using MySearchEngine.Core.Algorithm;
using Xunit;

namespace MySearchEngine.Core.Tests.Algorithm
{
    public class BloomFilterTests
    {
        [Fact]
        public void Should_AsExpected()
        {
            var bf = new BloomFilter(100);
            var result = bf.TryAdd("bloom filter");
            Assert.True(result);
            result = bf.TryAdd("bloom filter");
            Assert.False(result);
        }
    }
}
