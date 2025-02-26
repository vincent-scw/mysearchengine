// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySearchEngine.AI;
using MySearchEngine.Core;

public class Program
{
    public static async Task Main(string[] args)
    {
        var build = CreateHostBuilder(args).Build();
        build.RunAsync();

        var gen = new DocTfIdfGenerator(build.Services.GetService<IRepository>());

        var flag = true;
        do {
            Console.WriteLine();
            Console.WriteLine("AI console is ready. Choose to start...");
            Console.WriteLine("1. Generate doc-term matrix.");
            Console.WriteLine("2. Load doc-term matrix.");
            Console.WriteLine("3. Start search.");
            Console.WriteLine("0. Close application.");

            var cmd = Console.ReadLine();
            
            switch (cmd)
            {
                case "1":
                    await gen.Generate();
                    Console.WriteLine("TF-IDF matrix generated.");
                    break;
                case "2":
                    Console.WriteLine("Start loading...");
                    await gen.Load();
                    Console.WriteLine("Matrix loaded.");
                    break;
                case "3":
                    var flag3 = true;
                    do
                    {
                        Console.WriteLine("Input doc id (should be 1-11238).");
                        var docId = Console.ReadLine();
                        try
                        {
                            var res = await gen.GetSimilarDocsAsync(int.Parse(docId));
                            Console.WriteLine($"Found by similarity for doc {docId}-{res.DocTitle}:");
                            for (int i = 0; i < res.SimilarDocs.Count(); i++)
                            {
                                Console.WriteLine($"  {i}> {res.SimilarDocs[i].DocId} - {res.SimilarDocs[i].Title}");
                                Console.WriteLine($"  {res.SimilarDocs[i].Url}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            flag3 = false;
                        }
                    } while (flag3);

                    break;
                case "0":
                    flag = false;
                    break;
            }
        }
        while (flag == true);

        Console.ReadLine();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((cxt, services) =>
            {
                services.AddScoped<IRepository, BinRepository>();
            });
}