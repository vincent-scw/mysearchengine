using System.Collections.Generic;
using System.Linq;

namespace MySearchEngine.Core.Analyzer.TokenFilters
{
    class StopWordTokenFilter : ITokenFilter
    {
        private readonly List<string> _stopWordList;
        public StopWordTokenFilter(IEnumerable<string> stopWordList)
        {
            _stopWordList = stopWordList.ToList();
        }

        public List<Token> Filter(List<Token> tokens)
        {
            return tokens.Where(x => !_stopWordList.Contains(x.Term)).ToList();
        }
    }
}
