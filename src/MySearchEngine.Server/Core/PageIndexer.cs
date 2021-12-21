using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Analyzer;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace MySearchEngine.Server.Core
{
    internal class PageIndexer
    {
        private readonly TextAnalyzer _textAnalyzer;
        private readonly InvertedIndex _invertedIndex;
        private readonly BinRepository _binRepository;

        private readonly ConcurrentDictionary<int, string> _termDictionary;
        private readonly ConcurrentDictionary<int, PageInfo> _pageDictionary;
        public PageIndexer(
            TextAnalyzer textAnalyzer, 
            InvertedIndex invertedIndex,
            BinRepository binRepository)
        {
            _textAnalyzer = textAnalyzer;
            _invertedIndex = invertedIndex;
            _binRepository = binRepository;

            _termDictionary = new ConcurrentDictionary<int, string>();
            _pageDictionary = new ConcurrentDictionary<int, PageInfo>();
        }

        public void Index(PageInfo page)
        {
            var tokens = _textAnalyzer.Analyze(page.Content);
            tokens.ForEach(t =>
            {
                _termDictionary.TryAdd(t.Id, t.Term);
                _invertedIndex.Index(t.Id, page.Id);
            });
            _pageDictionary.TryAdd(page.Id, page);

        }

        public async Task StoreDataAsync()
        {
            // Store term dict
            await _binRepository.StoreTermsAsync(_termDictionary);
            // Store page info
            await _binRepository.StorePagesAsync(_pageDictionary);
            _pageDictionary.Clear();

            // Store inverted index
            await _binRepository.StoreIndexAsync(_invertedIndex.TermPageMapping);

        }
    }
}
