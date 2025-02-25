namespace MySearchEngine.Core
{
    public class TermDocScore
    {
        public string Term { get; }
        public DocInfo DocInfo { get; }
        public double Score { get; }

        public TermDocScore(string term, DocInfo docInfo, double score)
        {
            Term = term;
            DocInfo = docInfo;
            Score = score;
        }
    }
}
