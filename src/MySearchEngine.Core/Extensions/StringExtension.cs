using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
