using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using MySearchEngine.Core;

namespace MySearchEngine.Repository
{
    public class DictionaryCrawledRepository : ICrawledRepository
    {
        private readonly IIdGenerator<int> _pageIdGenerator;
        private readonly ConcurrentDictionary<int, int> _repository = new ConcurrentDictionary<int, int>();

        public bool AddIfNew(Uri uri)
        {
            return _repository.TryAdd(uri.GetHashCode(), 1);
        }

        public bool Exists(Uri uri)
        {
            return _repository.ContainsKey(uri.GetHashCode());
        }
    }
}
