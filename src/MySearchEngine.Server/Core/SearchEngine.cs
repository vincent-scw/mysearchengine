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

        public List<TermDocScore> Search(string searchText, int size, int from)
        {
            var tokens = _textAnalyzer.Analyze(searchText);
            var indexedDocs = tokens.SelectMany(t =>
            {
                var term = t.Term;
                // Find indexed pages
                if (!_pageIndexer.TryGetIndexedPages(term, out List<TermInDoc> docs))
                    return new List<TermDocScore>();

                return docs.Select(p =>
                {
                    if (!_pageIndexer.TryGetPageInfo(p.DocId, out DocInfo pi))
                        return (TermDocScore)null;

                    // Use TF-IDF to calculate the score
                    return new TermDocScore(term, pi,
                        Tf_Idf.Calculate(
                            p.TermInDocCount, 
                            pi.TokenCount, 
                            _pageIndexer.GetTotalPagesCount(),
                            docs.Count));
                }).Where(x => x != null).ToList();
            }).ToList();

            // Sum up all token scores by page
            var ret = indexedDocs.GroupBy(ip => ip.DocInfo.Id)
                .Select(x =>
                {
                    var doc = x.First();
                    return new TermDocScore(doc.Term, doc.DocInfo, x.Sum(d => d.Score));
                }).ToList();

            ret.Sort(new ScoreComparer());

            return ret.Skip(from).Take(size).ToList();
        }

        private class ScoreComparer : Comparer<TermDocScore>
        {
            public override int Compare(TermDocScore x, TermDocScore y)
            {
                return y.Score.CompareTo(x.Score);
            }
        }
    }
}
