using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using System.Collections.Generic;

namespace MySearchEngine.Core
{
    public class AnalyzerBuilder
    {
        public static TextAnalyzer BuildTextAnalyzer(IIdGenerator<int> idGenerator, IEnumerable<string> stopWords)
        {
            return new TextAnalyzer(
                idGenerator,
                new List<ICharacterFilter>
                {
                    new HtmlElementFilter()
                },
                new SimpleTokenizer(), // id should be generated from term count
                new List<ITokenFilter>
                {
                    new LowercaseTokenFilter(),
                    new StemmerTokenFilter(),
                    new StopWordTokenFilter(stopWords)
                });
        }
    }
}
