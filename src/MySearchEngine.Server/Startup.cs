using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySearchEngine.Core.Analyzer;
using MySearchEngine.Core.Analyzer.CharacterFilters;
using MySearchEngine.Core.Analyzer.TokenFilters;
using MySearchEngine.Core.Analyzer.Tokenizers;
using MySearchEngine.Core.Utilities;
using MySearchEngine.Server.Indexer;
using Qctrl;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Grpc.Net.Client;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

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

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MySearchEngine.Server", Version = "v1" });
            });

            services.AddSingleton((sp) =>
                new QueueSvc.QueueSvcClient(GrpcChannel.ForAddress(Configuration.GetConnectionString("QueueService"),
                    new GrpcChannelOptions() {Credentials = ChannelCredentials.Insecure})));
            services.AddSingleton<IIdGenerator<int>, GlobalTermIdGenerator>();
            services.AddSingleton<TextAnalyzer>((sp) =>
            {
                var idGenerator = sp.GetRequiredService<IIdGenerator<int>>();
                var stopWordsStr = File.ReadAllText("..\\..\\res\\stop_words_english.json");
                return new TextAnalyzer(
                    new List<ICharacterFilter>
                    {
                        new HtmlElementFilter()
                    },
                    new SimpleTokenizer(idGenerator),
                    new List<ITokenFilter>
                    {
                        new LowercaseTokenFilter(),
                        new StemmerTokenFilter(),
                        new StopWordTokenFilter(JsonConvert.DeserializeObject<List<string>>(stopWordsStr))
                    });
            });
            services.AddSingleton<PageIndexer>();
            services.AddHostedService<IndexHostedService>();
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
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
