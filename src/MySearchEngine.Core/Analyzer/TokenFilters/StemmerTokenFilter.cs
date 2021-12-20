using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core.Algorithm;

namespace MySearchEngine.Core.Analyzer.TokenFilters
{
    class StemmerTokenFilter : ITokenFilter
    {
        public List<Token> Filter(List<Token> tokens)
        {
            var stemmer = new PorterStemmer();
            tokens.ForEach(x => x.Term = stemmer.StemWord(x.Term));

            var newTokens = tokens.GroupBy(x => x.Term).Select(x =>
            {
                var t = x.First();
                return x.Count() == 1 ? t : new Token(t.Id, t.Term)
                {
                    Positions = x.SelectMany(tk => tk.Positions).ToList()
                };
            });
            return newTokens.ToList();
        }
    }
}
