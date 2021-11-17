using System.Collections.Generic;

namespace MySearchEngine.Core
{
    internal class TrieNode
    {
        public char Char { get; }
        public string Data { get; }
        public int Length => Data.Length;
        public bool IsEndingChar { get; set; }
        public IDictionary<char, TrieNode> Children { get; private set; }
        public TrieNode Fail { get; set; }
        public int VisitedCount { get; set; }
        public bool IsRoot { get; }

        public TrieNode(char c, string previousData, bool isRoot = false)
        {
            Char = c;
            IsRoot = isRoot;
            Data = isRoot ? string.Empty : previousData + c;
            Children = new Dictionary<char, TrieNode>();
        }

        public TrieNode GetOrAppend(char next)
        {
            var lowered = char.ToLower(next);
            var found = Children.TryGetValue(lowered, out TrieNode tnext);
            if (!found)
            {
                tnext = new TrieNode(lowered, Data);
                Children.Add(lowered, tnext);
            }

            return tnext;
        }

        public TrieNode Find(char next)
        {
            var lowered = char.ToLower(next);
            Children.TryGetValue(lowered, out TrieNode tnext);
            return tnext;
        }
    }
}