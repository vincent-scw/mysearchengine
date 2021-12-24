using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;

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
                    // Don't use stop word to do search
                    // new StopWordTokenFilter(await _binRepository.ReadStopWordsAsync())
                });
        }

        public List<(PageInfo pageInfo, double score)> Search(string searchText, int size, int from)
        {
            var tokens = _textAnalyzer.Analyze(searchText);
            return null;
        }
    }
}
