using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MySearchEngine.QueueService
{
    class DefaultBackgroundQueue : IBackgroundQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
        public DefaultBackgroundQueue(int capacity)
        {
            BoundedChannelOptions options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }

        public async ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem)
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            Func<CancellationToken, ValueTask>? workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
