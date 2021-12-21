using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Analyzer;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.Server.Core
{
    internal class PageIndexer
    {
        private readonly TextAnalyzer _textAnalyzer;
        private readonly InvertedIndex _invertedIndex;
        private readonly BinRepository _binRepository;

        private readonly Dictionary<int, string> _termDictionary;
        private readonly Dictionary<int, PageInfo> _pageDictionary;
        private readonly SemaphoreSlim _semaphoreSlim;
        private int _newAfterStoreCount;
        public PageIndexer(
            TextAnalyzer textAnalyzer, 
            InvertedIndex invertedIndex,
            BinRepository binRepository)
        {
            _textAnalyzer = textAnalyzer;
            _invertedIndex = invertedIndex;
            _binRepository = binRepository;

            _termDictionary = new Dictionary<int, string>();
            _pageDictionary = new Dictionary<int, PageInfo>();
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _newAfterStoreCount = 0;
        }

        public void Index(PageInfo page, string content)
        {
            var tokens = _textAnalyzer.Analyze(content);
            _semaphoreSlim.Wait();
            try
            {
                _newAfterStoreCount++;

                tokens.ForEach(t =>
                {
                    _termDictionary.TryAdd(t.Id, t.Term);
                    _invertedIndex.Index(t.Id, page.Id);
                });
                _pageDictionary.TryAdd(page.Id, page);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task StoreDataAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (_newAfterStoreCount == 0)
                    return;

                // Store term dict
                await _binRepository.StoreTermsAsync(_termDictionary);
                // Store page info
                await _binRepository.StorePagesAsync(_pageDictionary);
                // Store inverted index
                await _binRepository.StoreIndexAsync(_invertedIndex.TermPageMapping);

                _newAfterStoreCount = 0;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
