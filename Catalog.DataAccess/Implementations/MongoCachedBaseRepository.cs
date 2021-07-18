using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Attributes;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.DataAccess.Implementations
{
    // TODO: Exception handling!!!
    public class MongoCachedBaseRepository<TRepoEntity> : ICachedBaseRepository<TRepoEntity> where TRepoEntity: MongoRepoEntity
    {
        private readonly ILogger<MongoCachedBaseRepository<TRepoEntity>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly ISendEndpointProvider  _sendEndpointProvider;
        private readonly string _repoEntityName;
        private readonly string _queueName;
        
        // ReSharper disable once MemberCanBePrivate.Global
        // Because Items should be available in derived repositories.
        protected readonly IMongoCollection<TRepoEntity> Items;
        
        public MongoCachedBaseRepository(ILogger<MongoCachedBaseRepository<TRepoEntity>> logger,
            IMongoOptions mongoOptions,
            IRedisCacheService cacheService,
            ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _cacheService = cacheService;
            _sendEndpointProvider = sendEndpointProvider;

            // Fill constants.
            _repoEntityName = typeof(TRepoEntity).Name;
            
            _queueName = string.Empty;
            var customAttributes = (BusCacheTopic[])typeof(TRepoEntity).GetCustomAttributes(typeof(BusCacheTopic), true);
            if (customAttributes.Length > 0)
            {
                var queueNameAttribute = customAttributes[0];
                _queueName = $"queue:{queueNameAttribute.Name}";
            }
            
            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.DatabaseName);

            Items = database.GetCollection<TRepoEntity>(typeof(TRepoEntity).Name);
        }
        
        public async Task<List<TRepoEntity>> GetAsync()
        {
            var redisResult = await _cacheService.GetAsync<List<TRepoEntity>>(_repoEntityName);
            if (redisResult != default)
            {
                return redisResult;
            }
            
            var dbResult = await (await Items.FindAsync(item => true)).ToListAsync();
            await RefreshCache("Get");

            return dbResult;
        }

        public async Task<TRepoEntity> GetAsync(string id) =>
            await (await Items.FindAsync(item => item.Id == id)).FirstOrDefaultAsync();

        public async Task<TRepoEntity> CreateAsync(TRepoEntity item)
        {
            // Assign unique id.
            item.Id = ObjectId.GenerateNewId().ToString();
            await Items.InsertOneAsync(item);

            await RefreshCache("Create");
            return item;
        }

        public async Task UpdateAsync(string id, TRepoEntity itemIn)
        {
            await Items.ReplaceOneAsync(item => item.Id == id, itemIn);
            await RefreshCache("Update");
        }

        public async Task RemoveAsync(TRepoEntity itemIn)
        {
            await Items.DeleteOneAsync(item => item.Id == itemIn.Id);
            await RefreshCache("Remove");
        }

        public async Task RemoveAsync(string id)
        {
            await Items.DeleteOneAsync(item => item.Id == id);
            await RefreshCache("RemoveById");
        }

        public async Task RemoveManyAsync(string[] ids)
        {
            await Items.DeleteManyAsync(item => ids.Contains(item.Id));
            await RefreshCache("RemoveMany");
        }

        /// <summary>
        /// Send signal to refresh cache using RabbitMQ.
        /// </summary>
        /// <param name="source">Source method.</param>
        private async Task RefreshCache(string source)
        {
            if (!string.IsNullOrEmpty(_queueName))
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_queueName));
                await endpoint.Send(new CacheSignal(source));
            }
        }
    }
}