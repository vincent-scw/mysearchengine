using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySearchEngine.Core.Analyzer;
using Qctrl;

namespace MySearchEngine.Server.Indexer
{
    internal class PageIndexer
    {
        private readonly TextAnalyzer _textAnalyzer;
        public PageIndexer(TextAnalyzer textAnalyzer)
        {
            _textAnalyzer = textAnalyzer;
        }

        public async Task IndexAsync(Message message)
        {
            var tokens = _textAnalyzer.Analyze(message.Body);
        }
    }
}
