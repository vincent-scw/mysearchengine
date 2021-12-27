using MySearchEngine.Core;
using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace MySearchEngine.Server.Core
{
    public class SearchEngine
    {
        private readonly IDocIndexer _docIndexer;
        
        public SearchEngine(
            IDocIndexer docIndexer)
        {
            _docIndexer = docIndexer;
        }

        public List<TermDocScore> Search(string searchText, int size, int from)
        {
            var textAnalyzer = AnalyzerBuilder.BuildTextAnalyzer(new IntegerIdGenerator(), new List<string>());
            var tokens = textAnalyzer.Analyze(searchText);
            var indexedDocs = tokens.SelectMany(t =>
            {
                var term = t.Term;
                // Find indexed pages
                if (!_docIndexer.TryGetIndexedDocs(term, out List<TermInDoc> docs))
                    return new List<TermDocScore>();

                return docs.Select(p =>
                {
                    if (!_docIndexer.TryGetDocInfo(p.DocId, out DocInfo pi))
                        return (TermDocScore)null;

                    // Use TF-IDF to calculate the score
                    return new TermDocScore(term, pi,
                        Tf_Idf.Calculate(
                            p.TermsInDoc, 
                            pi.TokenCount, 
                            _docIndexer.GetTotalDocCount(),
                            docs.Count));
                }).Where(x => x != null).ToList();
            }).ToList();

            // Sum up all token scores by page
            var ret = indexedDocs.GroupBy(ip => ip.DocInfo.DocId)
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
