using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.Core
{
    public class DocIndexer : IDocIndexer
    {
        private readonly IRepository _binRepository;

        private InvertedIndex _invertedIndex;

        private IDictionary<string, int> _termDictionary;
        private IDictionary<int, DocInfo> _docDictionary;
        private IIdGenerator<int> _termIdGenerator;
        private IIdGenerator<int> _docIdGenerator;
        private IEnumerable<string> _stopWords;

        private readonly SemaphoreSlim _semaphoreSlim;
        private int _newAfterStoreCount;
        public DocIndexer(
            IRepository binRepository)
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
            _stopWords = await _binRepository.ReadStopWordsAsync();

            _invertedIndex = new InvertedIndex(await _binRepository.ReadIndexAsync());
            _termIdGenerator = new GlobalTermIdGenerator(_termDictionary.Count, _termDictionary);
            _docIdGenerator = new IntegerIdGenerator(_docDictionary.Count);
        }

        public void Index(DocInfo doc, string content)
        {
            var textAnalyzer = AnalyzerBuilder.BuildTextAnalyzer(_termIdGenerator, _stopWords);
            var tokens = textAnalyzer.Analyze(content);
            doc.SetTokenCount(tokens.Count);
            if (doc.DocId < 0)
            {
                doc.SetId(_docIdGenerator.Next(null));
            }

            _semaphoreSlim.Wait();
            try
            {
                _newAfterStoreCount++;

                tokens.ForEach(t =>
                {
                    _termDictionary.TryAdd(t.Term, t.Id);
                    _invertedIndex.Index(t, doc.DocId);
                });
                _docDictionary.TryAdd(doc.DocId, doc);
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
