using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using System.Reflection;
using AzisFood.MQ.Abstractions.Attributes;
using AzisFood.MQ.Abstractions.Models;
using GreenPipes;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using AzisFood.DataEngine.Abstractions.Interfaces;

namespace Catalog.Worker.Consumers
{
    public class BatchCacheConsumer<T>  : IConsumer<Batch<BusSignal>>
    {
        private readonly ICacheOperator<T> _cacheOperator;
        private readonly TimeSpan _expiry = TimeSpan.FromDays(1);

        public BatchCacheConsumer(
            ICacheOperator<T> cacheOperator)
        {
            _cacheOperator = cacheOperator;
        }

        public async Task Consume(ConsumeContext<Batch<BusSignal>> context) =>
            await _cacheOperator.FullRecache(_expiry);
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