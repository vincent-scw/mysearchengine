namespace MySearchEngine.Core
{
    public class BinFile
    {
        /// <summary>
        /// The file path for inverted index
        /// </summary>
        public string Index { get; } = "index.bin";
        /// <summary>
        /// The file path for Term
        /// </summary>
        public string Term { get; } = "term.bin";
        /// <summary>
        /// The file path for Doc
        /// </summary>
        public string Doc { get; } = "doc.bin";
        /// <summary>
        /// The file path for term offset in inverted index file
        /// </summary>
        public string TermOffset { get; } = "term_offset.bin";
        /// <summary>
        /// The cvs file path for tf-idf matrix
        /// </summary>
        public string TfIdfMatrix { get; } = "tfidf_matrix.csv";
    }
}
