﻿using System;
using CorrelationId;
using EggPlantApi.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using static System.String;

namespace EggPlantApi
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

            if (!IsNullOrEmpty(elasticUri))
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithCorrelationId()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        AutoRegisterTemplate = true,
                        
                    })
                    .CreateLogger();
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddCorrelationId();
            services.AddSingleton(Configuration);
            services.Configure<RavenSettings>(options =>
            {
                options.Url = new [] {Configuration["EggPlantApiDBUrl"]};
                options.DefaultDatabase = Configuration["EggPlantApiDB"];
            });
            services.AddSingleton<DocumentStoreHolder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "X-Correlation-ID",
                UseGuidForCorrelationId = true,
                UpdateTraceIdentifier = false
            });

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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public static class CorrelationIdLoggerConfigurationExtensions
    {
        public static LoggerConfiguration WithCorrelationId(
            this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
        {
            if (loggerEnrichmentConfiguration == null) throw new ArgumentNullException(nameof(loggerEnrichmentConfiguration));
            return loggerEnrichmentConfiguration.With<CorrelationIdEnricher>();
        }

    }

    public class CorrelationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            propertyFactory.CreateProperty("CorrelationId", "asdadadasdafjasfljashfasljhfsa");
        }
    }
}
