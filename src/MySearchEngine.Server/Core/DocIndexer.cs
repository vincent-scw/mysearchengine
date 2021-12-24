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
    public class DocIndexer
    {
        private readonly BinRepository _binRepository;

        private TextAnalyzer _textAnalyzer;
        private InvertedIndex _invertedIndex;

        private IDictionary<string, int> _termDictionary;

        private IDictionary<int, DocInfo> _docDictionary;

        private readonly SemaphoreSlim _semaphoreSlim;
        private int _newAfterStoreCount;
        public DocIndexer(
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
            _docDictionary = await _binRepository.ReadDocsAsync();
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

        public void Index(DocInfo page, string content)
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
                _docDictionary.TryAdd(page.Id, page);
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
                await _binRepository.StoreDocsAsync(_docDictionary);
                // Store inverted index
                await _binRepository.StoreIndexAsync(_invertedIndex.TermPageMapping);

                _newAfterStoreCount = 0;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public int GetTotalDocCount()
        {
            return _docDictionary.Count;
        }

        public bool TryGetIndexedDocs(string term, out List<TermInDoc> pages)
        {
            if (_termDictionary.TryGetValue(term, out int termId))
                return _invertedIndex.TryGetIndexedDocs(termId, out pages);

            pages = null;
            return false;
        }

        public bool TryGetDocInfo(int pageId, out DocInfo pi)
        {
            return _docDictionary.TryGetValue(pageId, out pi);
        }
    }
}
