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
        private readonly BinRepository _binRepository;

        private InvertedIndex _invertedIndex;
        private IDictionary<int, string> _termDictionary;
        private IDictionary<int, PageInfo> _pageDictionary;
        private readonly SemaphoreSlim _semaphoreSlim;
        private int _newAfterStoreCount;
        public PageIndexer(
            TextAnalyzer textAnalyzer,
            BinRepository binRepository)
        {
            _textAnalyzer = textAnalyzer;
            _binRepository = binRepository;
            
            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _newAfterStoreCount = 0;

            InitAsync().Wait();
        }

        /// <summary>
        /// Read data from stored bin file
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            _termDictionary = await _binRepository.ReadTermsAsync();
            _pageDictionary = await _binRepository.ReadPagesAsync();
            _invertedIndex = new InvertedIndex(await _binRepository.ReadIndexAsync());
        }

        public void Index(PageInfo page, string content)
        {
            var tokens = _textAnalyzer.Analyze(content);
            page.TokenCount = tokens.Count;

            _semaphoreSlim.Wait();
            try
            {
                _newAfterStoreCount++;

                tokens.ForEach(t =>
                {
                    _termDictionary.TryAdd(t.Id, t.Term);
                    _invertedIndex.Index(t, page.Id);
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
