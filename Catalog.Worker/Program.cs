using System.IO;
using System.Reflection;
using AzisFood.CacheService.Redis.Extensions;
using AzisFood.DataEngine.Cache.CacheService.Extensions;
using AzisFood.DataEngine.MQ.Rabbit.Extensions;
using AzisFood.DataEngine.Postgres.Extensions;
using AzisFood.MQ.Rabbit.Extensions;
using Catalog.Core;
using Catalog.DataAccess.Models;
using Catalog.Worker.Consumers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Catalog.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    .AddJsonFile("appsettings.json");
                var configuration = configBuilder.Build();

                // Add open tracing with Jaeger
                services.AddJaeger(configuration);

                // Add Redis config
                services.AddRedisSupport(configuration);

                // Add MongoDb config
                // Add MongoDb config
                services
                    .AddPostgresSupport(configuration)
                    .UseCacheServiceAdapter()
                    .UseRabbitCacheEventHandler();

                // Register mappings
                services.AddMapper();


                // Registrations
                services.AddCoreServices();

                // Add RabbitMQ MassTransit
                services.AddRabbitMQSupport(configuration, config =>
                {
                    var tempServiceProvider = services.BuildServiceProvider();
                    // Add consumers and re-cache entities
                    config.AddConsumer<Product>(tempServiceProvider);
                    config.AddConsumer<Ingredient>(tempServiceProvider);
                    config.AddConsumer<Option>(tempServiceProvider);
                    config.AddConsumer<Category>(tempServiceProvider);
                    config.AddConsumer<IngredientDeleteBatchConsumer>(
                        typeof(IngredientDeleteBatchConsumerDefinition));
                });
            });
    }
}