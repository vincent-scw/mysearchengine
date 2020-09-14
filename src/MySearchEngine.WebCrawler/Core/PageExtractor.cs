using MySearchEngine.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MySearchEngine.WebCrawler.Tests")]

namespace MySearchEngine.WebCrawler.Core
{
    internal class PageExtractor : IPageExtractor
    {
        private static IDictionary<string, string> linkList = new Dictionary<string, string>
        {
            {"href=\"", "\""}
        };

        private static IDictionary<string, string> removeList = new Dictionary<string, string>
        {
            {"<![CDATA[", string.Empty},
            {"]]>", string.Empty},
            {"[&hellip;]",string.Empty},
            {"<style", "</style>"},
            {"<script", "</script>"},
            {"<option", "</option>"},
        };

        public (IEnumerable<string> links, string content) Extract(string htmlContent)
        {
            var (links, afterRemove) = ExtractFirst(htmlContent);
            var purified = Purify(afterRemove);

            return (links, purified);
        }

        private static (List<string> links, string afterRemove) ExtractFirst(string htmlContent)
        {
            var links = new List<string>();
            var sb = new StringBuilder();

            var index = 0;
            var foundList = htmlContent.Search(linkList.Keys.Union(removeList.Keys));

            foreach (var (position, value) in foundList)
            {
                var startIndex = position + value.Length;
                if (linkList.Keys.Contains(value))
                {
                    // Get links
                    var endIndex = htmlContent.IndexOf(linkList[value], startIndex);
                    links.Add(htmlContent[startIndex..endIndex]);
                }
                else
                {
                    if (index > position)
                    {
                        continue;
                    }
                    // Remove not display content
                    sb.Append(htmlContent[index..position].TrimEnd());

                    var removeValue = removeList[value];
                    var removeTo = removeValue == string.Empty ? position + value.Length : htmlContent.IndexOf(removeValue, startIndex) + removeValue.Length;
                    sb.Append(' ');

                    index = removeTo;
                }
            }

            if (index < htmlContent.Length - 1)
            {
                sb.Append(htmlContent[index..]);
            }

            return (links, sb.ToString());
        }

        private static string Purify(string htmlContent)
        {
            // Remove all html tags
            var foundList = htmlContent.Search(new string[] { "<" });
            var sb = new StringBuilder();

            var index = 0;
            foreach (var (position, value) in foundList)
            {
                if (index > position)
                    continue;

                sb.Append(htmlContent[index..position].TrimEnd());
                sb.Append(' ');

                index = htmlContent.IndexOf(">", position + value.Length) + 1;
            }

            return sb.ToString();
        }
    }
}
