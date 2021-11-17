using System.Collections.Generic;

namespace MySearchEngine.Core.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Search in text with given patterns
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="patterns">Patterns</param>
        /// <returns>Search results with position</returns>
        public static IEnumerable<(int position, string value)> Search(this string text, IEnumerable<string> patterns)
        {
            var acMatcher = new AcPatternMatcher(patterns);
            return acMatcher.Match(text);
        }

        /// <summary>
        /// Visit text, get word
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<(string term, int visitedCount)> Visit(this string text)
        {
            var docTrie = new DocumentTrie();
            docTrie.BuildTrie(text);

            var list = Visit(docTrie.Root);

            return list;
        }

        private static IEnumerable<(string term, int visitedCount)> Visit(TrieNode node)
        {
            var list = new List<(string term, int visitedCount)>();
            foreach (var n in node.Children)
            {
                var cn = n.Value;
                if (cn.IsEndingChar)
                {
                    list.Add((cn.Data, cn.VisitedCount));
                }

                list.AddRange(Visit(cn));
            }

            return list;
        }
    }
}
