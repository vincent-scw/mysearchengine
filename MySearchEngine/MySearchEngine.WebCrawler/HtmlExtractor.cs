using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using String.Search;
using String.Search.Extensions;

namespace MySearchEngine.WebCrawler
{
    internal class HtmlExtractor
    {
        private static IDictionary<string, string> linkList = new Dictionary<string, string>
        {
            {"href=\"", "\""}
        };

        private static IDictionary<string, string> removeList = new Dictionary<string, string>
        {
            {"<style", "</style>"},
            {"<script", "</script>"},
            {"<option", "</option>"},
        };

        public void Extract(string htmlContent)
        {
            var links = new List<string>();
            var sb = new StringBuilder();

            var index = 0;
            var foundList = htmlContent.Search(linkList.Keys.Union(removeList.Keys));

            foreach (var f in foundList)
            {
                var startIndex = f.position + f.value.Length;
                if (linkList.Keys.Contains(f.value))
                {
                    var endIndex = htmlContent.IndexOf(linkList[f.value], startIndex);
                    links.Add(htmlContent[startIndex..endIndex]);
                }
                else
                {
                    sb.Append(htmlContent[index..f.position]);

                    var removeValue = removeList[f.value];
                    index = htmlContent.IndexOf(removeValue, startIndex) + removeValue.Length;
                }
            }

            if (index < htmlContent.Length - 1)
            {
                sb.Append(htmlContent[index..]);
            }

            var str = sb.ToString();
        }
    }
}
