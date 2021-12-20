using System.Collections.Generic;

namespace MySearchEngine.Core.Analyzer.TokenFilters
{
    public interface ITokenFilter
    {
        List<Token> Filter(List<Token> tokens);
    }
}
