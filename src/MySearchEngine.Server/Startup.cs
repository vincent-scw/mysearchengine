using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MySearchEngine.Core;
using MySearchEngine.Server.BackgroundServices;
using Qctrl;
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

            services.AddCors(opt => opt.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.Configure<BinFile>(Configuration.GetSection(nameof(BinFile)));
            services.AddSingleton((sp) =>
                new QueueSvc.QueueSvcClient(GrpcChannel.ForAddress(Configuration.GetConnectionString("QueueService"),
                    new GrpcChannelOptions() {Credentials = ChannelCredentials.Insecure})));
            services.AddSingleton<IDocIndexer, DocIndexer>();
            services.AddSingleton<SearchEngine>();
            services.AddTransient<IRepository, BinRepository>();
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

            app.UseCors("AllowAll");
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
