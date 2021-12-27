using MySearchEngine.Core;
using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Utilities;
using System;
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

        public List<SearchResultItem> Search(string searchText, int size, int from)
        {
            var textAnalyzer = AnalyzerBuilder.BuildTextAnalyzer(new IntegerIdGenerator(), new List<string>());
            // Analyze text
            var tokens = textAnalyzer.Analyze(searchText);
            // Find indexed docs
            var indexedDocsWithScore = tokens.SelectMany(CalculateTokenScore).ToList();

            // Sum up all token scores by page
            var ret = indexedDocsWithScore.GroupBy(ip => ip.DocInfo.DocId)
                .Select(x =>
                {
                    var doc = x.First();
                    var item = new SearchResultItem
                    {
                        DocId = doc.DocInfo.DocId,
                        Title = doc.DocInfo.Title,
                        Url = doc.DocInfo.Url,
                        Score = Math.Round(x.Sum(d => d.Score), 4),
                        Explain = x.Select(tds => new ItemExplain
                        {
                            Term = tds.Term,
                            ScoreInDoc = Math.Round(tds.Score, 4)
                        }).ToList()
                    };

                    return item;
                }).ToList();

            ret.Sort(new ScoreComparer());

            return ret.Skip(from).Take(size).ToList();
        }

        private List<TermDocScore> CalculateTokenScore(Token token)
        {
            var term = token.Term;
            // Find indexed pages
            if (!_docIndexer.TryGetIndexedDocs(term, out List<TermInDoc> docs))
                return new List<TermDocScore>();

            // Score term in each doc
            var tds = docs.Select(p =>
            {
                if (!_docIndexer.TryGetDocInfo(p.DocId, out DocInfo di))
                    return (TermDocScore) null;

                // Use TF-IDF to calculate the score
                return new TermDocScore(term, di,
                    Tf_Idf.Calculate(
                        p.Count,
                        di.TokenCount,
                        _docIndexer.GetTotalDocCount(),
                        docs.Count));
            }).Where(x => x != null).ToList();

            return tds;
        }
    }
}
