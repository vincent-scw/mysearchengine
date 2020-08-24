using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MySearchEngine.Algorithms.Tests")]

namespace MySearchEngine.Algorithms
{
    internal class AcPatternMatcher
    {
        internal TrieNode Root { get; private set; }

        public AcPatternMatcher(IEnumerable<string> terms)
        {
            Root = new TrieNode('~', -1); // Any meaningless char is OK here
            foreach (var term in terms)
            {
                Insert(term);
            }

            BuildFailurePointer();
        }

        public bool Exists(string pattern)
        {
            var p = Root;
            foreach (var c in pattern)
            {
                p = p.Find(c);
                if (p == null)
                {
                    // Not found
                    return false;
                }
            }

            return p.IsEndingChar;
        }

        public IEnumerable<(int position, string value)> Match(string text)
        {
            var ret = new List<(int, string)>();

            var n = text.Length;
            var p = Root;
            for (var i = 0; i < n; i++)
            {
                var currentChar = char.ToLower(text[i]);
                while (!p.Children.ContainsKey(currentChar) && p != Root)
                {
                    p = p.Fail;
                }

                if (p.Children.ContainsKey(currentChar))
                {
                    p = p.Children[currentChar];
                }

                if (p == null) p = Root;

                var tmp = p;
                while (tmp != Root)
                {
                    if (tmp.IsEndingChar)
                    {
                        var pos = i - tmp.Length + 1;
                        ret.Add((pos, text.Substring(pos, tmp.Length)));
                    }

                    tmp = tmp.Fail;
                }
            }

            return ret;
        }

        public string Replace(string text, char replaceWith = '*')
        {
            string newText = text;
            var n = text.Length;
            var p = Root;
            for (var i = 0; i < n; i++)
            {
                var currentChar = char.ToLower(text[i]);
                while (!p.Children.ContainsKey(currentChar) && p != Root)
                {
                    p = p.Fail;
                }

                if (p.Children.ContainsKey(currentChar))
                {
                    p = p.Children[currentChar];
                }

                if (p == null) p = Root;

                var tmp = p;
                while (tmp != Root)
                {
                    if (tmp.IsEndingChar)
                    {
                        var pos = i - tmp.Length + 1;
                        newText = newText.Remove(pos, tmp.Length).Insert(pos, new string(replaceWith, tmp.Length));
                    }

                    tmp = tmp.Fail;
                }
            }

            return newText;
        }

        private void Insert(string term)
        {
            var p = Root;
            foreach (var c in term)
            {
                p = p.GetOrAppend(c);
            }

            p.IsEndingChar = true;
        }

        private void BuildFailurePointer()
        {
            var queue = new Queue<TrieNode>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                foreach (var pc in p.Children.Values)
                {
                    if (p == Root)
                    {
                        pc.Fail = Root;
                    }
                    else
                    {
                        var q = p.Fail;
                        while (q != null)
                        {
                            q.Children.TryGetValue(pc.Data, out TrieNode qc);
                            if (qc != null)
                            {
                                pc.Fail = qc;
                                break;
                            }
                            q = q.Fail;
                        }

                        if (q == null)
                            pc.Fail = Root;
                    }

                    queue.Enqueue(pc);
                }
            }
        }
    }
}
