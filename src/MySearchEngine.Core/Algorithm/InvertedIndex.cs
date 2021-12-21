using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MySearchEngine.Core.Algorithm
{
    public class InvertedIndex
    {
        private readonly ConcurrentDictionary<int, List<int>> _termPageMapping;
        public IReadOnlyDictionary<int, List<int>> TermPageMapping => _termPageMapping;
        public InvertedIndex(ConcurrentDictionary<int, List<int>> termPageMapping)
        {
            _termPageMapping = termPageMapping ?? new ConcurrentDictionary<int, List<int>>();
        }

        public void Index(int termId, int pageId)
        {
            if (_termPageMapping.TryGetValue(termId, out List<int> list))
                list.Add(pageId);
            else
            {
                list = new List<int> {pageId};
                _termPageMapping.TryAdd(termId, list);
            }
        }
    }
}
