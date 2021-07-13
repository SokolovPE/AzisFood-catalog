using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Interfaces;
using Catalog.DataAccess.Models;
using Hangfire;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Catalog.DataAccess.Implementations
{
    // TODO: Exception handling!!!
    public class MongoCachedBaseRepository<TRepoEntity> : ICachedBaseRepository<TRepoEntity> where TRepoEntity: MongoRepoEntity
    {
        private readonly ILogger<MongoCachedBaseRepository<TRepoEntity>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly TimeSpan _expiry;
        private readonly string _repoEntityName;
        protected readonly IMongoCollection<TRepoEntity> Items;
        
        public MongoCachedBaseRepository(ILogger<MongoCachedBaseRepository<TRepoEntity>> logger, IMongoOptions mongoOptions, IRedisCacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
            
            // Fill constants.
            _expiry = TimeSpan.FromDays(1);
            _repoEntityName = typeof(TRepoEntity).Name;
            
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
            var cacheSetResult = await _cacheService.SetAsync(_repoEntityName, dbResult, _expiry);
            if (!cacheSetResult)
            {
                _logger.LogWarning($"Unable to set {_repoEntityName} to cache");
            }

            return dbResult;
        }

        public async Task<TRepoEntity> GetAsync(string id) =>
            await (await Items.FindAsync(item => item.Id == id)).FirstOrDefaultAsync();

        public async Task<TRepoEntity> CreateAsync(TRepoEntity item)
        {
            // Assign unique id.
            item.Id = ObjectId.GenerateNewId().ToString();
            await Items.InsertOneAsync(item);
            BackgroundJob.Enqueue(() => RefreshCache());
            return item;
        }

        public async Task UpdateAsync(string id, TRepoEntity itemIn)
        {
            await Items.ReplaceOneAsync(item => item.Id == id, itemIn);
            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public async Task RemoveAsync(TRepoEntity itemIn)
        {
            await Items.DeleteOneAsync(item => item.Id == itemIn.Id);
            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public async Task RemoveAsync(string id)
        {
            await Items.DeleteOneAsync(item => item.Id == id);
            BackgroundJob.Enqueue(() => RefreshCache());
        }

        public async Task RemoveManyAsync(string[] ids)
        {
            await Items.DeleteManyAsync(item => ids.Contains(item.Id));
            BackgroundJob.Enqueue(() => RefreshCache());
        }

        // ReSharper disable once MemberCanBePrivate.Global
        // Required by hangfire.
        public async Task RefreshCache()
        {
            await _cacheService.RemoveAsync(_repoEntityName);
            var items = await (await Items.FindAsync(i => true)).ToListAsync();
            var cacheSetResult = await _cacheService.SetAsync(_repoEntityName, items, _expiry);
            if (!cacheSetResult)
            {
                _logger.LogWarning($"Unable to refresh {_repoEntityName} cache");
            }
        }
    }
}