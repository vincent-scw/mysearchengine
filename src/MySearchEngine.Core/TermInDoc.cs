namespace MySearchEngine.Core
{
    public class TermInDoc
    {
        public int TermId { get; }
        public int DocId { get; }
        /// <summary>
        /// Term occurs count in doc
        /// </summary>
        public int TermInDocCount { get; }

        public TermInDoc(int termId, int docId, int termInDocCount)
        {
            TermId = termId;
            DocId = docId;
            TermInDocCount = termInDocCount;
        }
    }
}
