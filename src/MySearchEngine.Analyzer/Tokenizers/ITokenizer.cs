using System.Collections.Generic;

namespace MySearchEngine.Analyzer.Tokenizers
{
    public interface ITokenizer
    {
        IEnumerable<Token> Tokenize(string text);
    }
}
