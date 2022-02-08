using System;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Catalog.Worker.Consumers;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Worker;

public static class Extensions
{
    /// <summary>
    ///     Adds consumer to MassTransit and refreshes cache of entity
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public static void AddConsumer<T>(
        this IServiceCollectionBusConfigurator configurator, IServiceProvider serviceProvider)
    {
        var cacheOperator = serviceProvider.GetRequiredService<ICacheOperator<T>>();
        cacheOperator.FullRecache(TimeSpan.FromDays(1));
        configurator.AddConsumer<BatchCacheConsumer<T>>(typeof(BatchCacheConsumerDefinition<T>));
    }
}