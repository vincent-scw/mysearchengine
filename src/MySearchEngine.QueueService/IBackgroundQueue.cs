using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.QueueService
{
    interface IBackgroundQueue
    {
        ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem);
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
