using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core.Algorithm;
using Xunit;

namespace MySearchEngine.Core.Tests.Algorithm
{
    public class AcPatternMatcherTests
    {
        [Fact]
        public void TrieExists_Should_ReturnExpected()
        {
            var trie = new AcPatternMatcher(new List<string>
            {
                "how",
                "hi",
                "her",
                "hello",
                "so",
                "see"
            });

            Assert.True(trie.Exists("how"));
            Assert.True(trie.Exists("hi"));
            Assert.True(trie.Exists("HER"));
            Assert.True(trie.Exists("Hello"));
            Assert.True(trie.Exists("so"));
            Assert.True(trie.Exists("See"));

            Assert.False(trie.Exists("HERE"));
            Assert.False(trie.Exists("he"));
            Assert.False(trie.Exists("she"));
        }

        [Fact]
        public void TrieSearch_Should_ReturnExpected()
        {
            var trie = new AcPatternMatcher(new List<string>
            {
                "abcd",
                "bcd",
                "BC",
                "c",
                "bc cd"
            });

            var ret = trie.Match("dd abdbc cd zzzz").ToArray();

            Assert.Equal(4, ret.Length);
            Assert.Equal((6, "bc"), ret[0]);
            Assert.Equal((7, "c"), ret[1]);
            Assert.Equal((9, "c"), ret[2]);
            Assert.Equal((6, "bc cd"), ret[3]);
        }
    }
}
