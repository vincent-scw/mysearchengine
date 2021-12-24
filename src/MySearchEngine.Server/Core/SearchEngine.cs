using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core;

namespace MySearchEngine.Server.Core
{
    public class SearchEngine
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
                if (!_pageIndexer.TryGetIndexedPages(term, out List<TermInDoc> docs))
                    return new List<(PageInfo, double)>();

                return docs.Select(p =>
                {
                    if (!_pageIndexer.TryGetPageInfo(p.DocId, out PageInfo pi))
                        return ((PageInfo)null, 0);

                    // Use TF-IDF to calculate the score
                    return (pi,
                        Tf_Idf.Calculate(p.TermInDocCount, pi.TokenCount, _pageIndexer.GetTotalPagesCount(), docs.Count));
                }).Where(x => x.pi != null).ToList();
            }).ToList();

            // Sum up all token scores by page
            var ret = indexedPages.GroupBy(ip => ip.Item1.Id)
                .Select(x =>
                {
                    var page = x.First();
                    return (page.Item1, x.Sum(d => d.Item2));
                }).ToList();

            ret.Sort(new ScoreComparer());

            return ret.Skip(from).Take(size).ToList();
        }

        private class ScoreComparer : Comparer<(PageInfo, double)>
        {
            public override int Compare((PageInfo, double) x, (PageInfo, double) y)
            {
                return y.Item2.CompareTo(x.Item2);
            }
        }
    }
}
