using System.Collections.Generic;
using System.Linq;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;

namespace MySearchEngine.Core.Analyzer
{
    public class TextAnalyzer
    {
        private readonly IReadOnlyList<ICharacterFilter> _characterFilters;
        private readonly ITokenizer _tokenizer;
        private readonly IReadOnlyList<ITokenFilter> _tokenFilters;

        public TextAnalyzer(
            IReadOnlyList<ICharacterFilter> filters,
            ITokenizer tokenizer,
            IReadOnlyList<ITokenFilter> tokenFilters)
        {
            _characterFilters = filters;
            _tokenizer = tokenizer;
            _tokenFilters = tokenFilters;
        }

        public List<Token> Analyze(string text)
        {
            var filteredText = text;
            if (_characterFilters?.Count > 0)
            {
                filteredText = _characterFilters.Aggregate(filteredText, (current, characterFilter) => characterFilter.Filter(current));
            }

            var tokens = _tokenizer.Tokenize(filteredText).ToList();

            if (_tokenFilters?.Count > 0)
            {
                tokens = _tokenFilters.Aggregate(tokens, (current, tokenFilter) => tokenFilter.Filter(current));
            }

            return tokens;
        }
    }
}
