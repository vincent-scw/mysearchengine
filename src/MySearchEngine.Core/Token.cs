using System.Collections.Generic;

namespace MySearchEngine.Core
{
    public class Token
    {
        public int Id { get; private set; }
        public string Term { get; set; }
        public List<int> Positions { get; set; }
        /// <summary>
        /// Count of terms in current doc
        /// </summary>
        public int TermsInDoc => Positions.Count;

        public Token(string term)
        {
            Term = term;
            Positions = new List<int>();
        }

        public void SetId(int id)
        {
            Id = id;
        }
    }
}
