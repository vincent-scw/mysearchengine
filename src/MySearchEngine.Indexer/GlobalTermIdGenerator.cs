using MySearchEngine.Core;
using System.Collections.Generic;

namespace MySearchEngine.Indexer
{
    class GlobalTermIdGenerator : IIdGenerator<int>
    {
        private readonly IntegerIdGenerator _idGenerator;
        private readonly Dictionary<string, int> _termIdDict;

        public GlobalTermIdGenerator()
        {
            _idGenerator = new IntegerIdGenerator();
            _termIdDict = new Dictionary<string, int>();
        }

        public int Next(string parameter)
        {
            // parameter is 'term'
            return _termIdDict.ContainsKey(parameter) ? _termIdDict[parameter] : _idGenerator.Next(null);
        }
    }
}
