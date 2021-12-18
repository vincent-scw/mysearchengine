using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Qctrl;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MySearchEngine.QueueService
{
    class QueueSvcImpl : QueueSvc.QueueSvcBase
    {
        private readonly ConcurrentQueue<Message> _queue;
        private readonly ILogger _logger;
        public QueueSvcImpl(ILogger logger)
        {
            _queue = new ConcurrentQueue<Message>();
            _logger = logger;
        }

        public override Task<Result> Enqueue(Message request, ServerCallContext context)
        {
            _queue.Enqueue(request);
            _logger.LogInformation($"New message (id:{request.Id}) enqueued. {_queue.Count} messages in the queue.");
            return Task.FromResult(new Result() {Succeed = true});
        }

        public override Task<Message> Read(Empty request, ServerCallContext context)
        {
            _queue.TryPeek(out Message msg);
            return Task.FromResult(msg);
        }

        public override Task<Result> Ack(Message request, ServerCallContext context)
        {
            // simply dequeue
            _queue.TryDequeue(out Message _);
            _logger.LogInformation($"Message (id: {request.Id}) dequeued. {_queue.Count} messages remaining.");
            return Task.FromResult(new Result() {Succeed = true});
        }
    }
}
