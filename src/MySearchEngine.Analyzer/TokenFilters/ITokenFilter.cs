using System.Collections.Generic;
using MySearchEngine.Analyzer.Tokenizers;

namespace MySearchEngine.Analyzer.TokenFilters
{
    public interface ITokenFilter
    {
        List<Token> Filter(List<Token> tokens);
    }
}
