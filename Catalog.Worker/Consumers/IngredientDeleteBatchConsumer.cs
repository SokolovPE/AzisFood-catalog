using System;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Core.Services.Interfaces;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Newtonsoft.Json;

namespace Catalog.Worker.Consumers
{
    public class IngredientDeleteBatchConsumer: IConsumer<Batch<BusSignal>>
    {
        private readonly IProductService _productService;
        private readonly ICacheOperator<Ingredient> _ingredientCacheOperator;
        private readonly ICacheOperator<Product> _productCacheOperator;

        public IngredientDeleteBatchConsumer(IProductService productService,
            ICacheOperator<Ingredient> ingredientCacheOperator, ICacheOperator<Product> productCacheOperator)
        {
            _productService = productService;
            _ingredientCacheOperator = ingredientCacheOperator;
            _productCacheOperator = productCacheOperator;
        }
        public async Task Consume(ConsumeContext<Batch<BusSignal>> context)
        {
            // TODO: Deserialize of string? what? 
            var idsToRemove = context.Message
                .Select(p => JsonConvert.DeserializeObject<string>(p.Message.Payload));
            var ingredientIds = idsToRemove as string[] ?? idsToRemove.ToArray();
            if (ingredientIds.Any())
            {
                await _productService.DeleteIngredients(ingredientIds);
                await _ingredientCacheOperator.FullRecache(TimeSpan.FromDays(1));
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
}