namespace MySearchEngine.Core
{
    public class TermInDoc
    {
        public int TermId { get; }
        public int DocId { get; }
        /// <summary>
        /// Term occurs count in doc
        /// </summary>
        public int Count { get; }

        public TermInDoc(int termId, int docId, int count)
        {
            TermId = termId;
            DocId = docId;
            Count = count;
        }
    }
}
