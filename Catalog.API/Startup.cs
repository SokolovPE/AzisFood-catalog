using System.IO;
using AzisFood.CacheService.Redis.Extensions;
using AzisFood.DataEngine.Mongo.Extensions;
using AzisFood.MQ.Rabbit.Extensions;
using Catalog.Extensions;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;

namespace Catalog
{
    /// <summary>
    /// Startup of application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Constructs startup
        /// </summary>
        /// <param name="configuration">Configuration of application</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }
        
        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services">Collection of services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // Register mappings
            services.AddMapper();
            
            // Add open tracing with Jaeger
            services.AddOpenTracing();
            services.AddSingleton<ITracer>(sp =>
            {
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory).WithSender(new UdpSender())
                    .Build();
                var tracer = new Tracer.Builder(serviceName)
                    // The constant sampler reports every span
                    .WithSampler(new ConstSampler(true))
                    // LoggingReporter prints every reported span to the logging framework
                    .WithReporter(reporter)
                    .Build();
                return tracer;
            });
            services.Configure<HttpHandlerDiagnosticOptions>(options =>
                options.OperationNameResolver =
                    request => $"{request.Method.Method}: {request?.RequestUri?.AbsoluteUri}");
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Catalog.API", Version = "v1"});
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Catalog.API.xml");
                c.IncludeXmlComments(filePath);
            });
            
            // Add RabbitMQ MassTransit
            services.AddRabbitMQSupport(Configuration);
            
            // Add MongoDb config
            services.AddMongoDBSupport(Configuration);
            
            // Add Redis config
            services.AddRedisSupport(Configuration);
            
            // Registrations
            services.AddCoreServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        /// <summary>
        /// Configure services
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}