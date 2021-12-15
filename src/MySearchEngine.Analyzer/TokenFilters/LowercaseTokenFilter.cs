using MySearchEngine.Analyzer.Tokenizers;
using System.Collections.Generic;

namespace MySearchEngine.Analyzer.TokenFilters
{
    class LowercaseTokenFilter : ITokenFilter
    {
        public List<Token> Filter(List<Token> tokens)
        {
            tokens.ForEach(t => t.Term = t.Term.ToLower());
            return tokens;
        }
    }
}
