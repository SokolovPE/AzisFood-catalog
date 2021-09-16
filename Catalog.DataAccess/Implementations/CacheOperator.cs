using System;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Extensions;
using Catalog.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Catalog.DataAccess.Implementations
{
    public class CacheOperator<T> : ICacheOperator<T>
    {
        private readonly ILogger<CacheOperator<T>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly IBaseRepository<T> _repository;
        private static readonly string EntityName = typeof(T).Name;

        public CacheOperator(ILogger<CacheOperator<T>> logger,
            IRedisCacheService cacheService,
            IBaseRepository<T> repository)
        {
            _logger = logger;
            _cacheService = cacheService;
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task FullRecache(TimeSpan expiry, bool asHash = true)
        {
            await _cacheService.RemoveAsync(EntityName);
            var items = (await _repository.GetAsync()).ToList();
            
            if (asHash)
            {
                await _cacheService.HashSetAsync(items, CommandFlags.None);
            }
            else
            {
                var cacheSetResult = await _cacheService.SetAsync(EntityName, items, expiry, CommandFlags.None);
                if (!cacheSetResult)
                {
                    _logger.LogWarning($"Unable to refresh {EntityName} cache");
                    throw new Exception($"Unable to refresh {EntityName} cache");
                }
            }

            _logger.LogInformation($"Successfully refreshed {EntityName} cache");
        }
    }
}