using System;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Reflection;
using Catalog.DataAccess.Attributes;
using GreenPipes;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace Catalog.Worker.Consumers
{
    public class BatchCacheConsumer<T>  : IConsumer<Batch<CacheSignal>>
    {
        private readonly ILogger<BatchCacheConsumer<T>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly IBaseRepository<Ingredient> _repository;
        private const string EntityName = nameof(Ingredient);
        private readonly TimeSpan _expiry = TimeSpan.FromDays(1);

        public BatchCacheConsumer(ILogger<BatchCacheConsumer<T>> logger,
            IRedisCacheService cacheService,
            IBaseRepository<Ingredient> repository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<Batch<CacheSignal>> context)
        {
            await _cacheService.RemoveAsync(EntityName);
            var items = await _repository.GetAsync();
            var cacheSetResult = await _cacheService.SetAsync(EntityName, items, _expiry, CommandFlags.None);
            if (!cacheSetResult)
            {
                _logger.LogWarning($"Unable to refresh {EntityName} cache");
                throw new Exception($"Unable to refresh {EntityName} cache");
            }

            _logger.LogInformation($"Successfully refreshed {EntityName} cache");
        }
    }
    
    public class BatchCacheConsumerDefinition<T> :
        ConsumerDefinition<BatchCacheConsumer<T>>
    {
        public BatchCacheConsumerDefinition()
        {
            string queueName;
            var customAttributes = (BusCacheTopic[])typeof(T).GetCustomAttributes(typeof(BusCacheTopic), true);
            if (customAttributes.Length > 0)
            {
                var queueNameAttribute = customAttributes[0];
                queueName = queueNameAttribute.Name;
            }
            else
            {
                throw new CustomAttributeFormatException(
                    $"Entity {typeof(T).Name} missing {nameof(BusCacheTopic)} attribute");
            }
            Endpoint(x =>
            {
                x.Name = queueName;
                x.PrefetchCount = 1000;
            });
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<BatchCacheConsumer<T>> consumerConfigurator)
        {
            // https://masstransit-project.com/advanced/batching.html
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(1000)
                .SetTimeLimit(5000));
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
        }
    }
}