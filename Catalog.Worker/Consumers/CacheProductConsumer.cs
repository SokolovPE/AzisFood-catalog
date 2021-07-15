using System;
using System.Globalization;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.Worker.Consumers
{
    /// <summary>
    /// Consumer for cache refreshing of type Product.
    /// </summary>
    public class CacheProductConsumer : IConsumer<Batch<CacheSignal>>
    {
        private readonly ILogger<CacheProductConsumer> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly IBaseRepository<Product> _repository;
        private const string EntityName = nameof(Product);
        private static readonly TimeSpan Expiry = TimeSpan.FromDays(1);

        public CacheProductConsumer(ILogger<CacheProductConsumer> logger, IRedisCacheService cacheService, IBaseRepository<Product> repository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<Batch<CacheSignal>> context)
        {
            await _cacheService.RemoveAsync(EntityName);
            var items = await _repository.GetAsync();
            var cacheSetResult = await _cacheService.SetAsync(EntityName, items, Expiry, CommandFlags.None);
            if (!cacheSetResult)
            {
                _logger.LogWarning($"Unable to refresh {EntityName} cache");
                throw new Exception($"Unable to refresh {EntityName} cache");
            }
            else
            {
                _logger.LogInformation($"Successfully refreshed {EntityName} cache");
            }
        }
    }
    
    public class CacheProductConsumerDefinition :
        ConsumerDefinition<CacheProductConsumer>
    {
        public CacheProductConsumerDefinition()
        {
            Endpoint(x =>
            {
                x.Name = "recache.product";
                x.PrefetchCount = 1000;
            });
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<CacheProductConsumer> consumerConfigurator)
        {
            // https://masstransit-project.com/advanced/batching.html
            consumerConfigurator.Options<BatchOptions>(options => options
                .SetMessageLimit(1000)
                .SetTimeLimit(5000));
            endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500));
        }
    }
}