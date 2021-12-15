using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Analyzer.Tokenizers;

namespace MySearchEngine.Analyzer.TokenFilters
{
    class StopWordTokenFilter : ITokenFilter
    {
        private readonly List<string> _stopWordList;
        public StopWordTokenFilter(IEnumerable<string> stopWordList)
        {
            _stopWordList = stopWordList.ToList();
        }

        public void Filter(List<Token> tokens)
        {
            throw new System.NotImplementedException();
        }
    }
}
