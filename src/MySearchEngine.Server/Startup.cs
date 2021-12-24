using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySearchEngine.Core.Algorithm;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using MySearchEngine.Server.BackgroundServices;
using MySearchEngine.Server.Core;
using Newtonsoft.Json;
using Qctrl;
using System.Collections.Generic;
using System.IO;

namespace MySearchEngine.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MySearchEngine.Server", Version = "v1" });
            });

            services.Configure<BinPath>(Configuration.GetSection(nameof(BinPath)));
            services.AddSingleton((sp) =>
                new QueueSvc.QueueSvcClient(GrpcChannel.ForAddress(Configuration.GetConnectionString("QueueService"),
                    new GrpcChannelOptions() {Credentials = ChannelCredentials.Insecure})));
            services.AddSingleton((sp) =>
            {
                var stopWordsStr = File.ReadAllText("..\\..\\res\\stop_words_english.json");
                return new TextAnalyzer(
                    new List<ICharacterFilter>
                    {
                        new HtmlElementFilter()
                    },
                    new SimpleTokenizer(new GlobalTermIdGenerator()),
                    new List<ITokenFilter>
                    {
                        new LowercaseTokenFilter(),
                        new StemmerTokenFilter(),
                        new StopWordTokenFilter(JsonConvert.DeserializeObject<List<string>>(stopWordsStr))
                    });
            });
            services.AddSingleton<PageIndexer>();
            services.AddSingleton((sp) =>
            {
                return new InvertedIndex(new Dictionary<int, List<(int, int)>>());
            });
            services.AddTransient<BinRepository>();
            services.AddHostedService<IndexHostedService>();
            services.AddHostedService<StorageHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MySearchEngine.Server v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", async context => await context.Response.WriteAsync("OK"));
                endpoints.MapControllers();
            });
        }
    }
}
