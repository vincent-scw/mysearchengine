using System.Collections.Concurrent;

namespace MySearchEngine.Core.Utilities
{
    public class GlobalTermIdGenerator : IIdGenerator<int>
    {
        private readonly IntegerIdGenerator _idGenerator;
        private readonly ConcurrentDictionary<string, int> _termIdDict;

        public GlobalTermIdGenerator()
        {
            _idGenerator = new IntegerIdGenerator();
            _termIdDict = new ConcurrentDictionary<string, int>();
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
