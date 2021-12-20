using System.Collections.Generic;

namespace MySearchEngine.Core.Analyzer.Tokenizers
{
    public interface ITokenizer
    {
        IEnumerable<Token> Tokenize(string text);
    }
}
