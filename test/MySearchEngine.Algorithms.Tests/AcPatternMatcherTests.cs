using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MySearchEngine.Algorithms.Tests
{
    [TestClass]
    public class AcPatternMatcherTests
    {
        [TestMethod]
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

            Assert.IsTrue(trie.Exists("how"));
            Assert.IsTrue(trie.Exists("hi"));
            Assert.IsTrue(trie.Exists("HER"));
            Assert.IsTrue(trie.Exists("Hello"));
            Assert.IsTrue(trie.Exists("so"));
            Assert.IsTrue(trie.Exists("See"));

            Assert.IsFalse(trie.Exists("HERE"));
            Assert.IsFalse(trie.Exists("he"));
            Assert.IsFalse(trie.Exists("she"));
        }

        [TestMethod]
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

            Assert.AreEqual(4, ret.Length);
            Assert.AreEqual((6, "bc"), ret[0]);
            Assert.AreEqual((7, "c"), ret[1]);
            Assert.AreEqual((9, "c"), ret[2]);
            Assert.AreEqual((6, "bc cd"), ret[3]);
        }
    }
}
