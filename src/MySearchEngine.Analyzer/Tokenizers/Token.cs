using System.Collections.Generic;

namespace MySearchEngine.Analyzer.Tokenizers
{
    public struct Token
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public List<int> Positions { get; set; }

        public Token(int id, string term)
        {
            Id = id;
            Term = term;
            Positions = new List<int>();
        }
    }
}
