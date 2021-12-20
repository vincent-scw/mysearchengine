using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Qctrl;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MySearchEngine.QueueService
{
    sealed class QueuedHostedService : BackgroundService
    {
        private readonly HostConfiguration _config;

        public QueuedHostedService(
            HostConfiguration config) =>
            (_config) = (config);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"{nameof(QueuedHostedService)} is running on {_config.Host}:{_config.ControlPort}.");
            return ProcessQueueAsync(stoppingToken);
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var server = new Server()
            {
                Services = { QueueSvc.BindService(new QueueSvcImpl()) },
                Ports = { new ServerPort(_config.Host, _config.ControlPort, ServerCredentials.Insecure)}
            };
            server.Start();
            Console.WriteLine("gRPC server started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                // keep server running
            }

            Console.WriteLine($"{nameof(QueuedHostedService)} is shutting down.");
            await server.ShutdownAsync();
        }
    }
}
