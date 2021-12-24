using System.Collections.Concurrent;
using System.Collections.Generic;
using MySearchEngine.Core.Analyzer;

namespace MySearchEngine.Core.Algorithm
{
    public class InvertedIndex
    {
        private readonly Dictionary<int, List<(int, int)>> _termPageMapping;
        public IReadOnlyDictionary<int, List<(int pageId, int termCount)>> TermPageMapping => _termPageMapping;
        public InvertedIndex(IDictionary<int, List<(int, int)>> termPageMapping)
        {
            _termPageMapping = new Dictionary<int, List<(int, int)>>(termPageMapping);
        }

        public void Index(Token token, int pageId)
        {
            var termId = token.Id;
            if (_termPageMapping.TryGetValue(termId, out List<(int, int)> list))
                list.Add((pageId, token.Positions.Count));
            else
            {
                list = new List<(int, int)> {(pageId, token.Positions.Count)};
                _termPageMapping.TryAdd(termId, list);
            }
        }

        public bool TryGetIndexedPages(int termId, out List<(int pageId, int termCount)> pages)
        {
            return _termPageMapping.TryGetValue(termId, out pages);
        }
    }
}
