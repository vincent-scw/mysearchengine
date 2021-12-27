namespace MySearchEngine.Core
{
    public class TermInDoc
    {
        public int TermId { get; }
        public int DocId { get; }
        /// <summary>
        /// Term occurs count in doc
        /// </summary>
        public int TermsInDoc { get; }

        public TermInDoc(int termId, int docId, int termsInDoc)
        {
            TermId = termId;
            DocId = docId;
            TermsInDoc = termsInDoc;
        }
    }
}
