using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Qctrl;

namespace MySearchEngine.QueueService
{
    sealed class QueuedHostedService : BackgroundService
    {
        private readonly HostConfiguration _config;
        private readonly ILogger<QueuedHostedService> _logger;

        public QueuedHostedService(
            HostConfiguration config,
            ILogger<QueuedHostedService> logger) =>
            (_config, _logger) = (config, logger);

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(QueuedHostedService)} is running.");
            return ProcessQueueAsync(stoppingToken);
        }

        private async Task ProcessQueueAsync(CancellationToken cancellationToken)
        {
            var server = new Server()
            {
                Services = { QueueSvc.BindService(new QueueSvcImpl(_logger)) },
                Ports = { new ServerPort(_config.Host, _config.ControlPort, ServerCredentials.Insecure)}
            };
            server.Start();
            _logger.LogInformation("gRPC server started.");

            while (!cancellationToken.IsCancellationRequested)
            {
                // keep server running
            }

            _logger.LogInformation($"{nameof(QueuedHostedService)} is shutting down.");
            await server.ShutdownAsync();
        }
    }
}
