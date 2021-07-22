using System;
using System.Linq;
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
    public class BatchCacheConsumer<T>  : IConsumer<Batch<BusSignal>>
    {
        private readonly ILogger<BatchCacheConsumer<T>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly IBaseRepository<T> _repository;
        private static readonly string EntityName = typeof(T).Name;
        private readonly TimeSpan _expiry = TimeSpan.FromDays(1);

        public BatchCacheConsumer(ILogger<BatchCacheConsumer<T>> logger,
            IRedisCacheService cacheService,
            IBaseRepository<T> repository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<Batch<BusSignal>> context)
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
            var customAttributes = (BusTopic[]) typeof(T).GetCustomAttributes(typeof(BusTopic), true);
            var busTopic = customAttributes.FirstOrDefault();
            if (busTopic != null && !string.IsNullOrEmpty(busTopic.Name) &&
                busTopic.Events.Contains(EventType.Recache))
            {
                queueName = busTopic.FullName(EventType.Recache.ToString(), false);
            }
            else
            {
                throw new CustomAttributeFormatException(
                    $"Bus topic is missing or operation Recache is not permitted for {typeof(T).Name}");
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