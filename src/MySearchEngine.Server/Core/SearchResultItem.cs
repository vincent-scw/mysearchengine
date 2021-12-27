using System.Collections.Generic;

namespace MySearchEngine.Server.Core
{
    public class SearchResultItem
    {
        public int DocId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public double Score { get; set; }
        public List<ItemExplain> Explain { get; set; }

        public SearchResultItem()
        {
            Explain = new List<ItemExplain>();
        }
    }

    public class ItemExplain
    {
        public string Term { get; set; }
        public double ScoreInDoc { get; set; }
    }

    public class ScoreComparer : Comparer<SearchResultItem>
    {
        public override int Compare(SearchResultItem x, SearchResultItem y)
        {
            return y.Score.CompareTo(x.Score);
        }
    }
}
