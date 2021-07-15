using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Catalog.DataAccess.Implementations;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Catalog.Worker.Consumers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Catalog.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                {
                    //services.AddHostedService<Worker>();
                    var configBuilder = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("appsettings.json");
                    var configuration = configBuilder.Build();
                    
                    // Add Redis config.
                    services.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));
                    services.AddSingleton<IRedisOptions>(sp =>
                        sp.GetRequiredService<IOptions<RedisOptions>>().Value);
                    services.AddSingleton<IRedisCacheService, RedisCacheService>();
                    // Add MongoDb config.
                    services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));
                    services.AddSingleton<IMongoOptions>(sp =>
                        sp.GetRequiredService<IOptions<MongoOptions>>().Value);
                    
                    // Add RabbitMQ MassTransit.
                    services.AddMassTransit(config =>
                    {
                        config.AddConsumer<CacheProductConsumer>(typeof(CacheProductConsumerDefinition));
                        config.UsingRabbitMq((ctx, cfg) =>
                        {
                            cfg.Host(configuration.GetValue<string>("MassTransitOptions:ConnectionString"));
                            cfg.ConfigureEndpoints(ctx);
                        });
                        //config.AddConsumer<CacheProductConsumer>(typeof(CacheProductConsumerDefinition));
                    });
                    services.AddMassTransitHostedService();
                    services.AddTransient(typeof(IBaseRepository<>), typeof(MongoBaseRepository<>));
                });
    }
}