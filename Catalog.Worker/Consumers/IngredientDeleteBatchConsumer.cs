using System;
using System.Linq;
using System.Threading.Tasks;
using AzisFood.CacheService.Redis.Interfaces;
using AzisFood.DataEngine.Abstractions.Interfaces;
using AzisFood.MQ.Abstractions.Models;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Models;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Catalog.Worker.Consumers;

public class IngredientDeleteBatchConsumer : IConsumer<Batch<BusSignal>>
{
    private readonly ICacheOperator<Ingredient> _ingredientCacheOperator;
    private readonly ICacheOperator<Product> _productCacheOperator;
    private readonly IProductService _productService;
    private readonly IRedisCacheService _redisCacheService;

    public IngredientDeleteBatchConsumer(IProductService productService,
        ICacheOperator<Ingredient> ingredientCacheOperator, ICacheOperator<Product> productCacheOperator,
        IRedisCacheService redisCacheService)
    {
        _productService = productService;
        _ingredientCacheOperator = ingredientCacheOperator;
        _productCacheOperator = productCacheOperator;
        _redisCacheService = redisCacheService;
    }

    public async Task Consume(ConsumeContext<Batch<BusSignal>> context)
    {
        var idsToRemove = context.Message
            .Select(p => JsonConvert.DeserializeObject<Guid>(p.Message.Payload));
        var ingredientIds = idsToRemove as Guid[] ?? idsToRemove.ToArray();
        if (ingredientIds.Any())
        {
            await _productService.DeleteIngredients(ingredientIds);
            var keys = ingredientIds.Select(x => new RedisValue(x.ToString())).ToArray();
            await _redisCacheService.HashRemoveManyAsync<Ingredient>(keys, CommandFlags.None);
            await _productCacheOperator.FullRecache(TimeSpan.FromDays(1));
        }
    }
}

public class IngredientDeleteBatchConsumerDefinition :
    ConsumerDefinition<IngredientDeleteBatchConsumer>
{
    public IngredientDeleteBatchConsumerDefinition()
    {
        const string queueName = "deleted.ingredient";

        Endpoint(x =>
        {
            x.Name = queueName;
            x.PrefetchCount = 1000;
        });
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<IngredientDeleteBatchConsumer> consumerConfigurator)
    {
        // https://masstransit-project.com/advanced/batching.html
        consumerConfigurator.Options<BatchOptions>(options => options
            .SetMessageLimit(1000)
            .SetTimeLimit(5000));
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
    }
}