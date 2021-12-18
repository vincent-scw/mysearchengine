using Xunit;

namespace MySearchEngine.Core.Tests
{
    public class BooleanFilterTests
    {
        [Fact]
        public void Should_AsExpected()
        {
            var bf = new BooleanFilter(100);
            var result = bf.TryAdd("boolean filter");
            Assert.True(result);
            result = bf.TryAdd("boolean filter");
            Assert.False(result);
        }
    }
}
