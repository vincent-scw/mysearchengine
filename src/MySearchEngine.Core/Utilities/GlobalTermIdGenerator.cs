using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MySearchEngine.Core.Utilities
{
    public class GlobalTermIdGenerator : IIdGenerator<int>
    {
        private readonly IntegerIdGenerator _idGenerator;
        private readonly ConcurrentDictionary<string, int> _termIdDict;

        public GlobalTermIdGenerator(int seed, IDictionary<string, int> termIdDict)
        {
            _idGenerator = new IntegerIdGenerator(seed);
            _termIdDict = new ConcurrentDictionary<string, int>(termIdDict);
        }

        public int Next(string parameter)
        {
            // parameter is 'term'
            if (_termIdDict.TryGetValue(parameter, out int termId))
            {
                return termId;
            }

            // Not thread safe
            termId = _idGenerator.Next(null);
            _termIdDict.TryAdd(parameter, termId);
            return termId;
        }
    }
}
