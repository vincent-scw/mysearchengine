using MySearchEngine.Core.Algorithm;
using Xunit;

namespace MySearchEngine.Core.Tests.Algorithm
{
    public class Tf_IdfTests
    {
        [Fact]
        public void Should_AsExpected()
        {
            var result = Tf_Idf.Calculate(3, 100, 10_000_000, 1_000);
            Assert.Equal(0.12d, result);
        }
    }
}
