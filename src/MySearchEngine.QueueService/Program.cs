using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MySearchEngine.QueueService
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Arguments required. \"[host] [port]\"");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(new HostConfiguration()
                    {
                        Host = args[0],
                        ControlPort = Convert.ToInt32(args[1])
                    });

                    services.AddHostedService<QueuedHostedService>();
                });
        }
    }
}
