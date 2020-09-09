using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace MySearchEngine.Repository
{
    public class DictionaryCrawledRepository : ICrawledRepository
    {
        private readonly ConcurrentDictionary<int, byte> _repository = new ConcurrentDictionary<int, byte>();

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
