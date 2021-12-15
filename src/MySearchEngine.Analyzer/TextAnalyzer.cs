using System;
using System.Collections.Generic;
using MySearchEngine.Analyzer.CharacterFilters;
using MySearchEngine.Analyzer.TokenFilters;
using MySearchEngine.Analyzer.Tokenizers;

namespace MySearchEngine.Analyzer
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

        public IEnumerable<Token> Analyze(string text)
        {
            var analyzedText = text;
            if (_characterFilters?.Count > 0)
            {
                foreach (var characterFilter in _characterFilters)
                {
                    analyzedText = characterFilter.Filter(analyzedText);
                }
            }

            return null;
        }
    }
}
