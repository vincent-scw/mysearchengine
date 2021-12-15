using System.Threading;

namespace MySearchEngine.Core
{
    public class IntegerIdGenerator : IIdGenerator<int>
    {
        private int _currentId;
        public IntegerIdGenerator(int seed = 0)
        {
            _currentId = seed;
        }

        public int Next(string parameter)
        {
            return Interlocked.Increment(ref _currentId);
        }
    }
}
