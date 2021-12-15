using System.Collections.Generic;
using MySearchEngine.Analyzer.Tokenizers;

namespace MySearchEngine.Analyzer.TokenFilters
{
    public interface ITokenFilter
    {
        void Filter(List<Token> tokens);
    }
}
