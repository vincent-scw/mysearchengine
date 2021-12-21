using System.Collections.Generic;

namespace MySearchEngine.Core.Analyzer.TokenFilters
{
    public class LowercaseTokenFilter : ITokenFilter
    {
        public List<Token> Filter(List<Token> tokens)
        {
            tokens.ForEach(t => t.Term = t.Term.ToLower());
            return tokens;
        }
    }
}
