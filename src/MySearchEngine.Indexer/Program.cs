using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySearchEngine.Core.Utilities;
using Qctrl;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;

namespace MySearchEngine.Indexer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Arguments required. \"[host] [port]\"");
                return;
            }

            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton((sp) =>
                        new QueueSvc.QueueSvcClient(new Channel(args[0], Convert.ToInt32(args[1]), ChannelCredentials.Insecure)));
                    services.AddSingleton<IIdGenerator<int>, GlobalTermIdGenerator>();
                    services.AddSingleton<TextAnalyzer>((sp) =>
                    {
                        var idGenerator = sp.GetRequiredService<IIdGenerator<int>>();
                        return new TextAnalyzer(
                            new List<ICharacterFilter>
                            {

                            },
                            new SimpleTokenizer(idGenerator),
                            new List<ITokenFilter>
                            {

                            });
                    });
                    services.AddHostedService<IndexHostedService>();
                });
        }
    }
}
