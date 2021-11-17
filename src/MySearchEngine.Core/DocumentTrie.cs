namespace MySearchEngine.Core
{
    internal class DocumentTrie
    {
        private const string WORD_SPLIT = " .,!\"";
        internal TrieNode Root { get; private set; }

        public DocumentTrie()
        {
            Root = new TrieNode('~', string.Empty, true);
        }

        public void BuildTrie(string document)
        {
            var p = Root;
            for (int i = 0; i < document.Length; i++)
            {
                if (WORD_SPLIT.IndexOf(document[i]) < 0)
                {
                    p = p.GetOrAppend(document[i]);
                }
                else
                {
                    p = Root;
                }

                if (i < document.Length - 1 && WORD_SPLIT.IndexOf(document[i + 1]) >= 0)
                {
                    p.IsEndingChar = true;
                    p.VisitedCount++;
                }
            }
        }
    }
}
