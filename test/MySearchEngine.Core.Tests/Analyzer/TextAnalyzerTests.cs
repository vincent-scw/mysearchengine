using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using Xunit;

namespace MySearchEngine.Core.Tests.Analyzer
{
    public class TextAnalyzerTests : IDisposable
    {
        private readonly TextAnalyzer _textAnalyzer;
        private readonly HttpClient _httpClient;
        public TextAnalyzerTests()
        {
            _textAnalyzer = new TextAnalyzer(
                new IntegerIdGenerator(0),
                new List<ICharacterFilter>
                {
                    new HtmlElementFilter()
                },
                new SimpleTokenizer(), // id should be generated from term count
                new List<ITokenFilter>
                {
                    new LowercaseTokenFilter(),
                    new StemmerTokenFilter()
                });

            _httpClient = new HttpClient();
        }

        [Fact]
        public async Task DownloadPage_Analyze_ShouldAsExpected()
        {
            var url = "https://www.differencebetween.com/difference-between-diploma-and-degree";
            var response = await _httpClient.GetAsync(url);

            var htmlContent = await response.Content.ReadAsStringAsync();
            var tokens = _textAnalyzer.Analyze(htmlContent);
            var schoolTerm = tokens.FirstOrDefault(x => x.Term == "school");
            
            Assert.NotNull(schoolTerm);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
