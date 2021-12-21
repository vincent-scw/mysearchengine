using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MySearchEngine.Core.Algorithm
{
    public class InvertedIndex
    {
        private readonly Dictionary<int, List<int>> _termPageMapping;
        public IReadOnlyDictionary<int, List<int>> TermPageMapping => _termPageMapping;
        public InvertedIndex(IDictionary<int, List<int>> termPageMapping)
        {
            _termPageMapping = new Dictionary<int, List<int>>(termPageMapping);
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
