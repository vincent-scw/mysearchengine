using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySearchEngine.WebCrawler.Core
{
    class PageExtractor : IPageExtractor
    {
        private const char ElementStart = '<';
        private const char ElementEnd = '>';
        private const string LinkElement = "a";
        private const string HrefAttr = " href=";
        private static readonly string[] EscapeElements = new[]
        {
            "script", 
            "option", 
            "style", 
            "meta", 
            "link", 
            "audio",
            "!DOCTYPE",
            "!--",
        };

        public (IEnumerable<string> links, string content) Extract(string htmlContent)
        {
            var links = new List<string>();
            var sb = new StringBuilder();

            var index = 0;
            var currentElement = string.Empty;
            while (index < htmlContent.Length)
            {
                if (htmlContent[index] == ElementStart)
                {
                    // Find element
                    var elementSb = new StringBuilder();
                    var elementGot = false;
                    do
                    {
                        index++;

                        if (char.IsWhiteSpace(htmlContent[index]) || htmlContent[index] == ElementEnd)
                        {
                            currentElement = elementSb.ToString();
                            elementGot = true;

                            if (currentElement == LinkElement)
                            {
                                // Get href in links
                                do
                                {
                                    if (htmlContent.Substring(index, HrefAttr.Length) == HrefAttr)
                                    {
                                        var startIndex = index + HrefAttr.Length + 1; // start index should be "
                                        var link = htmlContent.Substring(startIndex, htmlContent.IndexOf('\"', startIndex) - startIndex);
                                        links.Add(link);
                                    }

                                    index++;

                                } while (htmlContent[index] != ElementEnd);
                            }
                        }
                        else 
                        {
                            if (!elementGot)
                                elementSb.Append(htmlContent[index]);
                        }

                    } while (htmlContent[index] != ElementEnd);

                    
                }
                else if (htmlContent[index] == ElementEnd)
                {
                    index++;
                }
                else
                {
                    // Remove entire element
                    if (!EscapeElements.Any(x => x.Equals(currentElement, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        sb.Append(htmlContent[index]);
                    }

                    index++;
                }
            }

            return (links, sb.ToString());
        }
    }
}
