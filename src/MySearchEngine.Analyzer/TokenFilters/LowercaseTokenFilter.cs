using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySearchEngine.Analyzer.Tokenizers;

namespace MySearchEngine.Analyzer.TokenFilters
{
    class LowercaseTokenFilter : ITokenFilter
    {
        public void Filter(List<Token> tokens)
        {
            tokens.ForEach(t => t.Term = t.Term.ToLower());
        }
    }
}
