using System;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core.Algorithm;

namespace MySearchEngine.Server.Core
{
    class SearchEngine
    {
        private readonly PageIndexer _pageIndexer;

        private readonly TextAnalyzer _textAnalyzer;
        public SearchEngine(
            PageIndexer pageIndexer)
        {
            _pageIndexer = pageIndexer;
            _textAnalyzer = new TextAnalyzer(new List<ICharacterFilter>(),
                new SimpleTokenizer(new IntegerIdGenerator()),
                new List<ITokenFilter>
                {
                    new LowercaseTokenFilter(),
                    new StemmerTokenFilter(),
                    // Stop word filter is not included, so don't use stop word to do search
                    // new StopWordTokenFilter(await _binRepository.ReadStopWordsAsync())
                });
        }

        public List<(PageInfo pageInfo, double score)> Search(string searchText, int size, int from)
        {
            var tokens = _textAnalyzer.Analyze(searchText);
            var indexedPages = tokens.SelectMany(t =>
            {
                var term = t.Term;
                // Find indexed pages
                if (!_pageIndexer.TryGetIndexedPages(term, out List<(int pageId, int termCount)> pages))
                    return new List<(PageInfo, double)>();

                return pages.Select(p =>
                {
                    if (!_pageIndexer.TryGetPageInfo(p.pageId, out PageInfo pi))
                        return ((PageInfo)null, 0);

                    // Use TF-IDF to calculate the score
                    return (pi,
                        Tf_Idf.Calculate(p.termCount, pi.TokenCount, _pageIndexer.GetTotalPagesCount(), pages.Count));
                }).ToList();
            }).ToList();

            return indexedPages;
        }
    }
}
