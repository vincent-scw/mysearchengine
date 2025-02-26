using MySearchEngine.Core;

namespace MySearchEngine.AI
{
    public class SimilarDocuments
    {
        public int DocId { get; set; }
        public string DocTitle { get; set; }
        public string DocUrl { get; set; }
        public List<DocInfo> SimilarDocs { get; set; }
    }
}
