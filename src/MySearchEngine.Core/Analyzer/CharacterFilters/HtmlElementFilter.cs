using System;
using System.Linq;
using System.Text;

namespace MySearchEngine.Core.Analyzer.CharacterFilters
{
    class HtmlElementFilter : ICharacterFilter
    {
        private const char ElementStart = '<';
        private const char ElementEnd = '>';
        private const char Space = ' ';
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

        public string Filter(string originText)
        {
            // The return text should have the same length of origin text
            var sb = new StringBuilder(originText.Length);

            var index = 0;
            var currentElement = string.Empty;
            while (index < originText.Length)
            {
                if (originText[index] == ElementStart)
                {
                    // Find element
                    var elementSb = new StringBuilder();
                    var elementGot = false;
                    do
                    {
                        index++;
                        sb.Append(Space);

                        if (char.IsWhiteSpace(originText[index]) || originText[index] == ElementEnd)
                        {
                            currentElement = elementSb.ToString();
                            elementGot = true;
                        }
                        else
                        {
                            if (!elementGot)
                                elementSb.Append(originText[index]);
                        }

                    } while (originText[index] != ElementEnd);
                }
                else if (originText[index] == ElementEnd)
                {
                    index++;
                    sb.Append(Space);
                }
                else
                {
                    // Remove entire element
                    if (EscapeElements.Any(x => x.Equals(currentElement, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        index++;
                        sb.Append(Space);
                    }
                    else
                    {
                        sb.Append(originText[index++]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
