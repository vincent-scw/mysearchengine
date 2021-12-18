using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MySearchEngine.QueueService
{
    static class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(new HostConfiguration()
                    {
                        Host = "localhost",
                        ControlPort = 10024
                    });

                    services.AddHostedService<QueuedHostedService>();
                });
        }
    }
}
