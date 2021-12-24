namespace MySearchEngine.Server.Core
{
    public class DocInfo
    {
        public int DocId { get; }
        public string Title { get; }
        public string Url { get; }
        public int TokenCount { get; private set; }

        public DocInfo(int docId, string title, string url)
        {
            DocId = docId;
            Title = title;
            Url = url;
        }

        public DocInfo(int docId, string title, string url, int tokenCount)
            : this(docId, title, url)
        {
            TokenCount = tokenCount;
        }

        public void SetTokenCount(int count)
        {
            TokenCount = count;
        }
    }
}
