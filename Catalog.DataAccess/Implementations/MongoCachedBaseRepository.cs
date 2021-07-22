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
using Newtonsoft.Json;

namespace Catalog.DataAccess.Implementations
{
    // TODO: Tracing!!!
    public class MongoCachedBaseRepository<TRepoEntity> : ICachedBaseRepository<TRepoEntity>
        where TRepoEntity : MongoRepoEntity
    {
        private readonly ILogger<MongoCachedBaseRepository<TRepoEntity>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly string _repoEntityName;
        private readonly BusTopic _busTopic;

        // ReSharper disable once MemberCanBePrivate.Global
        // Because Items should be available in derived repositories
        protected readonly IMongoCollection<TRepoEntity> Items;

        public MongoCachedBaseRepository(ILogger<MongoCachedBaseRepository<TRepoEntity>> logger,
            IMongoOptions mongoOptions,
            IRedisCacheService cacheService,
            ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _cacheService = cacheService;
            _sendEndpointProvider = sendEndpointProvider;

            // Fill constants
            _repoEntityName = typeof(TRepoEntity).Name;

            var customAttributes = (BusTopic[]) typeof(TRepoEntity).GetCustomAttributes(typeof(BusTopic), true);
            _busTopic = customAttributes.FirstOrDefault();

            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.DatabaseName);

            Items = database.GetCollection<TRepoEntity>(typeof(TRepoEntity).Name);
        }

        public async Task<List<TRepoEntity>> GetAsync()
        {
            _logger.LogInformation($"Requested all {_repoEntityName} items");
            try
            {
                var redisResult = await _cacheService.GetAsync<List<TRepoEntity>>(_repoEntityName);
                if (redisResult != default)
                {
                    _logger.LogInformation(
                        $"Request of all {_repoEntityName} items returned {redisResult.Count} items");
                    return redisResult;
                }

                _logger.LogWarning($"Items of type {_repoEntityName} are not presented in cache");
                var dbResult = await (await Items.FindAsync(item => true)).ToListAsync();
                await SendEvent();

                _logger.LogInformation($"Request of all {_repoEntityName} items returned {dbResult.Count} items");
                return dbResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to return all {_repoEntityName} items");
                return default;
            }
        }

        public async Task<TRepoEntity> GetAsync(string id)
        {
            _logger.LogInformation($"Requested {_repoEntityName} with id: {id}");
            try
            {
                var redisResult =
                    (await _cacheService.GetAsync<List<TRepoEntity>>(_repoEntityName))?.FirstOrDefault(x => x.Id == id);
                if (redisResult != null)
                {
                    _logger.LogInformation($"Request of {_repoEntityName} with id: {id} succeeded");
                    return redisResult;
                }

                _logger.LogWarning($"Item of type {_repoEntityName}  with id: {id} is not presented in cache");
                var dbResult = await (await Items.FindAsync(item => item.Id == id)).FirstOrDefaultAsync();
                await SendEvent();
                _logger.LogInformation($"Request of {_repoEntityName} with id: {id} succeeded");
                return dbResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to return all {_repoEntityName} items");
                return default;
            }
        }

        public async Task<TRepoEntity> CreateAsync(TRepoEntity item)
        {
            _logger.LogInformation($"Requested creation of {_repoEntityName}: {JsonConvert.SerializeObject(item)}");
            try
            {
                // Assign unique id
                item.Id = ObjectId.GenerateNewId().ToString();
                await Items.InsertOneAsync(item);

                await SendEvent();
                _logger.LogInformation($"Requested creation of {_repoEntityName} succeeded");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to create {_repoEntityName}");
                return default;
            }
        }

        public async Task UpdateAsync(string id, TRepoEntity itemIn)
        {
            _logger.LogInformation(
                $"Requested update of {_repoEntityName} with id {id} with new value: {JsonConvert.SerializeObject(itemIn)}");
            try
            {
                await Items.ReplaceOneAsync(item => item.Id == id, itemIn);
                await SendEvent();
                _logger.LogInformation($"Requested update of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to update {_repoEntityName}");
            }
        }

        public async Task RemoveAsync(TRepoEntity itemIn)
        {
            _logger.LogInformation($"Requested delete of {_repoEntityName}: {JsonConvert.SerializeObject(itemIn)}");
            try
            {
                await Items.DeleteOneAsync(item => item.Id == itemIn.Id);
                await SendEvent();
                _logger.LogInformation($"Requested delete of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete {_repoEntityName}");
            }
        }

        public async Task RemoveAsync(string id)
        {
            _logger.LogInformation($"Requested delete of {_repoEntityName} with id {id}");
            try
            {
                await Items.DeleteOneAsync(item => item.Id == id);
                await SendEvent();
                await SendEvent(eventType: EventType.Deleted, payload: id);
                _logger.LogInformation($"Requested delete of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete {_repoEntityName}");
            }
        }

        public async Task RemoveManyAsync(string[] ids)
        {
            _logger.LogInformation(
                $"Requested delete of multiple {_repoEntityName} with ids {JsonConvert.SerializeObject(ids)}");
            try
            {
                await Items.DeleteManyAsync(item => ids.Contains(item.Id));
                await SendEvent();
                await SendEvent(eventType: EventType.Deleted, payload: ids);
                _logger.LogInformation($"Requested delete of multiple {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete multiple {_repoEntityName}");
            }
        }

        /// <summary>
        /// Send signal to RabbitMQ
        /// </summary>
        /// <param name="source">Source method</param>
        /// <param name="eventType">Type of event</param>
        /// <param name="payload">Payload for signal</param>
        private async Task SendEvent([System.Runtime.CompilerServices.CallerMemberName]
            string source = "",
            EventType eventType = EventType.Recache,
            object payload = null)
        {
            _logger.LogInformation(
                $"Requested event {eventType.ToString()} push to Message Bus from {source} with payload: {JsonConvert.SerializeObject(payload)}");
            try
            {
                if (_busTopic == null || string.IsNullOrEmpty(_busTopic.Name) ||
                    !_busTopic.Events.Contains(eventType))
                {
                    _logger.LogWarning(
                        $"Bus topic is missing or operation {eventType.ToString()} is not permitted for {_repoEntityName}");
                    return;
                }

                var payloadJson = payload == null ? string.Empty : JsonConvert.SerializeObject(payload);
                var endpoint =
                    await _sendEndpointProvider.GetSendEndpoint(
                        new Uri(_busTopic.FullName(eventType.ToString())));
                await endpoint.Send(new BusSignal(source, payloadJson));
                _logger.LogInformation($"Event {eventType.ToString()} fired succeeded for {_repoEntityName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error during event push to Message Bus");
            }
        }
    }
}