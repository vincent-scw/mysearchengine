using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.Core.Tests
{
    [TestClass]
    public class DocumentTrieTests
    {
        private const string EnglishText = @"It’s a technique for building a computer program that learns from data.";

        [TestMethod]
        public void Build_DocumentTrie_Should_ReturnAsExpected()
        {
            var docTrie = new DocumentTrie();
            docTrie.BuildTrie(EnglishText);
        }
    }
}
