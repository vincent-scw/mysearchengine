using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MySearchEngine.Core.Tests")]

namespace MySearchEngine.Core
{
    internal class DocumentTrie
    {
        internal TrieNode Root { get; private set; }

        public DocumentTrie()
        {
            Root = new TrieNode('~', -1); // Any meaningless char is OK here
        }

        public void BuildTrie(string document)
        {
            var p = Root;
            for (int i = 0; i < document.Length; i++)
            {
                if (char.IsLetterOrDigit(document[i]))
                {
                    p = p.GetOrAppend(document[i]);
                    if (i < document.Length - 1 && !char.IsLetterOrDigit(document[i + 1]))
                    {
                        p.IsEndingChar = true;
                        p.VisitCount++;
                    }
                }
            }
        }
    }
}
