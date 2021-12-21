using System.Collections.Generic;
using MySearchEngine.Core.Utilities;

namespace MySearchEngine.Core.Analyzer.Tokenizers
{
    public class SimpleTokenizer : ITokenizer
    {
        private readonly IIdGenerator<int> _idGenerator;
        private readonly Dictionary<string, Token> _termTokenMapping;
        public SimpleTokenizer(IIdGenerator<int> idGenerator)
        {
            _idGenerator = idGenerator;
            _termTokenMapping = new Dictionary<string, Token>();
        }

        public IEnumerable<Token> Tokenize(string text)
        {
            // Tokenize by space
            var index = 0;
            var tokenStart = -1;
            while (index <= text.Length)
            {
                if (index == text.Length || ShouldEscape(text[index]))
                {
                    if (tokenStart >= 0)
                    {
                        var term = text[tokenStart..index];
                        AddToToken(term, tokenStart);
                        tokenStart = -1;
                    }

                }
                else if (index == 0 || (index > 0 && ShouldEscape(text[index - 1])))
                {
                    tokenStart = index;
                }

                index++;
            }

            return _termTokenMapping.Values;
        }

        private void AddToToken(string term, int index)
        {
            var token = _termTokenMapping.ContainsKey(term)
                ? _termTokenMapping[term]
                : new Token(_idGenerator.Next(term), term);

            token.Positions.Add(index);
            _termTokenMapping.TryAdd(term, token);
        }

        private static bool ShouldEscape(char c)
        {
            // Take it simple, tokenize letter and digit only
            return !char.IsLetterOrDigit(c);
        }
    }
}
