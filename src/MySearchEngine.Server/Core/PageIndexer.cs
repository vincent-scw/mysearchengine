using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MySearchEngine.Core;

namespace MySearchEngine.Server.Core
{
    public class PageIndexer
    {
        private readonly BinRepository _binRepository;

        private TextAnalyzer _textAnalyzer;
        private InvertedIndex _invertedIndex;

        private IDictionary<string, int> _termDictionary;

        private IDictionary<int, PageInfo> _pageDictionary;

        private readonly SemaphoreSlim _semaphoreSlim;
        private int _newAfterStoreCount;
        public PageIndexer(
            BinRepository binRepository)
        {
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

            _textAnalyzer = new TextAnalyzer(
                new List<ICharacterFilter>
                {
                    new HtmlElementFilter()
                },
                new SimpleTokenizer(new GlobalTermIdGenerator(_termDictionary.Count)), // id should be generated from term count
                new List<ITokenFilter>
                {
                    new LowercaseTokenFilter(),
                    new StemmerTokenFilter(),
                    new StopWordTokenFilter(await _binRepository.ReadStopWordsAsync())
                });
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
                    _termDictionary.TryAdd(t.Term, t.Id);
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

        public int GetTotalPagesCount()
        {
            return _pageDictionary.Count;
        }

        public bool TryGetIndexedPages(string term, out List<TermInDoc> pages)
        {
            if (_termDictionary.TryGetValue(term, out int termId))
                return _invertedIndex.TryGetIndexedPages(termId, out pages);

            pages = null;
            return false;
        }

        public bool TryGetPageInfo(int pageId, out PageInfo pi)
        {
            return _pageDictionary.TryGetValue(pageId, out pi);
        }
    }
}
