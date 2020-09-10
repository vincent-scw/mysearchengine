using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MySearchEngine.Algorithms.Extensions;
using MySearchEngine.WebCrawler.Models;

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
                    // Remove not display content
                    sb.Append(htmlContent[index..position]);

                    var removeValue = removeList[value];
                    index = htmlContent.IndexOf(removeValue, startIndex) + removeValue.Length;
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
                sb.Append(htmlContent[index..position]);

                index = htmlContent.IndexOf(">", position + value.Length) + 1;
            }

            return sb.ToString();
        }
    }
}
