using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Qctrl;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MySearchEngine.QueueService
{
    class QueueClientImpl : QueueClient.QueueClientBase
    {
        private readonly ConcurrentQueue<Message> _queue;
        private readonly ILogger _logger;
        public QueueClientImpl(ILogger logger)
        {
            _queue = new ConcurrentQueue<Message>();
            _logger = logger;
        }

        public override Task<Result> EnqueueAsync(Message request, ServerCallContext context)
        {
            _queue.Enqueue(request);
            _logger.LogInformation($"New message enqueued. {_queue.Count} messages in the queue.");
            return Task.FromResult(new Result() {Succeed = true});
        }

        public override Task<Message> ReadAsync(Empty request, ServerCallContext context)
        {
            _queue.TryPeek(out Message msg);
            return Task.FromResult(msg);
        }

        public override Task<Result> AckAsync(Message request, ServerCallContext context)
        {
            // simply dequeue
            _queue.TryDequeue(out Message _);
            _logger.LogInformation($"Message dequeued. {_queue.Count} messages remaining.");
            return Task.FromResult(new Result() {Succeed = true});
        }
    }
}
