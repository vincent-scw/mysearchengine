using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MySearchEngine.Core
{
    public class IntegerIdGenerator : IIdGenerator<int>
    {
        private int _currentId;
        public IntegerIdGenerator()
        {
            _currentId = 0;
        }

        public int Next()
        {
            return Interlocked.Increment(ref _currentId);
        }
    }
}
