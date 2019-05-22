using System;
using EggOrdersApi.Context;
using EggOrdersApi.Integration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace EggOrdersApi
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            var elasticUri = Configuration["ElasticSearchUri"];

            if (!string.IsNullOrEmpty(elasticUri))
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        AutoRegisterTemplate = true,
                        BufferLogShippingInterval = TimeSpan.FromSeconds(5),
                        MinimumLogEventLevel = LogEventLevel.Verbose
                    })
                    .CreateLogger();
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton(Configuration);
            services.Configure<RavenSettings>(options =>
            {
                options.Url = new[] { Configuration["EggOrdersApiDBUrl"] };
                options.DefaultDatabase = Configuration["EggOrdersApiDB"];
            });
            services.AddSingleton<DocumentStoreHolder>();

            services.AddSingleton<IRabbitMqPersistentConnection, RabbitMqPersistentConnection>();
            services.AddSingleton<IEventBus, EventBusRabbitMq>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var documentStore = app.ApplicationServices.GetService<DocumentStoreHolder>();
                documentStore.EnsureDatabaseExists();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            loggerFactory.AddSerilog();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
