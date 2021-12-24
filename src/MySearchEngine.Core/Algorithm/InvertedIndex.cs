using System.Collections.Generic;

namespace MySearchEngine.Core.Algorithm
{
    public class InvertedIndex
    {
        private readonly Dictionary<int, List<TermInDoc>> _termPageMapping;
        public IReadOnlyDictionary<int, List<TermInDoc>> TermPageMapping => _termPageMapping;
        public InvertedIndex(IDictionary<int, List<TermInDoc>> termPageMapping)
        {
            _termPageMapping = new Dictionary<int, List<TermInDoc>>(termPageMapping);
        }

        public void Index(Token token, int pageId)
        {
            var termId = token.Id;
            if (_termPageMapping.TryGetValue(termId, out List<TermInDoc> list))
                list.Add(new TermInDoc(termId, pageId, token.TermInDocCount));
            else
            {
                list = new List<TermInDoc> { new TermInDoc(termId, pageId, token.TermInDocCount)};
                _termPageMapping.TryAdd(termId, list);
            }
        }

        public bool TryGetIndexedPages(int termId, out List<TermInDoc> pages)
        {
            return _termPageMapping.TryGetValue(termId, out pages);
        }
    }
}
