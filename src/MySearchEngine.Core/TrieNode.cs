using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MySearchEngine.Algorithms.Tests")]

namespace MySearchEngine.Core
{
    internal class TrieNode
    {
        public char Data { get; }
        public int Length { get; }
        public bool IsEndingChar { get; set; }
        public IDictionary<char, TrieNode> Children { get; private set; }
        public TrieNode Fail { get; set; }

        public TrieNode(char data, int previousLength)
        {
            Data = data;
            Length = previousLength + 1;
            Children = new Dictionary<char, TrieNode>();
        }

        public TrieNode GetOrAppend(char next)
        {
            var lowered = char.ToLower(next);
            var found = Children.TryGetValue(lowered, out TrieNode tnext);
            if (!found)
            {
                tnext = new TrieNode(lowered, Length);
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