using Catalog.Core.Mappings;
using Catalog.Core.Models;
using Catalog.Core.Services.Implementations;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Sdk.Models;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace Catalog.Core
{
    /// <summary>
    /// Extensions for using AutoMapper
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Add Jaeger tracing to application
        /// </summary>
        /// <param name="serviceCollection">Collection of services</param>
        /// <param name="configuration">Config of application</param>
        public static void AddJaeger(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var config = configuration.GetSection("JaegerOptions").Get<JaegerOptions>() ??
                         new JaegerOptions() {SamplingRate = 0.2, LowerBound = 0.0};

            serviceCollection.AddOpenTracing(builder =>
            {
                builder.AddAspNetCore(o =>
                {
                    o.Hosting.OperationNameResolver = context => context.Request.Path.Value;
                    // Ignore root
                    o.Hosting.IgnorePatterns.Add(context => context.Request.Path == "/");
                    // Ignore swagger
                    o.Hosting.IgnorePatterns.Add(context =>
                        context.Request.Path.Value != null && context.Request.Path.Value.Contains("swagger"));
                });
            });

            serviceCollection.AddSingleton<ITracer>(sp =>
            {
                var serviceName = string.IsNullOrWhiteSpace(config.ServiceName)
                    ? sp.GetRequiredService<IWebHostEnvironment>().ApplicationName
                    : config.ServiceName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var sender = string.IsNullOrWhiteSpace(config.Host) || config.Port <= 0
                    ? new UdpSender()
                    : new UdpSender(config.Host, config.Port, 0);
                var reporter = new RemoteReporter.Builder().WithLoggerFactory(loggerFactory).WithSender(sender)
                    .Build();
                var sampler = new GuaranteedThroughputSampler(config.SamplingRate, config.LowerBound);
                var tracer = new Tracer.Builder(serviceName)
                    // The constant sampler reports every span
                    .WithSampler(sampler)
                    // LoggingReporter prints every reported span to the logging framework
                    .WithReporter(reporter)
                    .Build();
                return tracer;
            });
        }

        /// <summary>
        /// Add automapper to application
        /// </summary>
        /// <param name="serviceCollection">Collection of services</param>
        public static void AddMapper(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DefaultMappingProfile>();
            });
        }

        /// <summary>
        /// Add main services to application
        /// </summary>
        /// <param name="services">Collection of services</param>
        public static void AddCoreServices(this IServiceCollection services)
        {
            // Ingredient service
            services.AddTransient(typeof(IService<Ingredient, IngredientDto, IngredientRequestDto>),
                typeof(BaseService<Ingredient, IngredientDto, IngredientRequestDto>));
            
            // Option service
            services.AddTransient(typeof(IService<Option, OptionDto, OptionRequestDto>),
                typeof(BaseService<Option, OptionDto, OptionRequestDto>));
            
            // Category service
            services.AddTransient(typeof(IService<Category, CategoryDto, CategoryRequestDto>),
                typeof(BaseService<Category, CategoryDto, CategoryRequestDto>));
            
            // Product service
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IValidatorService<Product>, ProductValidatorService>();
        }
    }
}