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
using OpenTracing;
using OpenTracing.Tag;
using StackExchange.Redis;

namespace Catalog.DataAccess.Implementations
{
    public class MongoCachedBaseRepository<TRepoEntity> : ICachedBaseRepository<TRepoEntity>
        where TRepoEntity : MongoRepoEntity
    {
        private readonly ILogger<MongoCachedBaseRepository<TRepoEntity>> _logger;
        private readonly IRedisCacheService _cacheService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ITracer _tracer;
        private readonly string _repoEntityName;
        private readonly BusTopic _busTopic;

        // ReSharper disable once MemberCanBePrivate.Global
        // Because Items should be available in derived repositories
        protected readonly IMongoCollection<TRepoEntity> Items;

        public MongoCachedBaseRepository(ILogger<MongoCachedBaseRepository<TRepoEntity>> logger,
            IMongoOptions mongoOptions,
            IRedisCacheService cacheService,
            ISendEndpointProvider sendEndpointProvider,
            ITracer tracer)
        {
            _logger = logger;
            _cacheService = cacheService;
            _sendEndpointProvider = sendEndpointProvider;
            _tracer = tracer;

            // Fill constants
            _repoEntityName = typeof(TRepoEntity).Name;

            var customAttributes = (BusTopic[]) typeof(TRepoEntity).GetCustomAttributes(typeof(BusTopic), true);
            _busTopic = customAttributes.FirstOrDefault();

            var client = new MongoClient(mongoOptions.ConnectionString);
            var database = client.GetDatabase(mongoOptions.DatabaseName);

            Items = database.GetCollection<TRepoEntity>(typeof(TRepoEntity).Name);
        }

        public async Task<IEnumerable<TRepoEntity>> GetAsync() =>
            await Get(false);

        public async Task<IEnumerable<TRepoEntity>> GetHashAsync() =>
            await Get();

        public async Task<TRepoEntity> GetAsync(string id) =>
            await Get(id, false);

        public async Task<TRepoEntity> GetHashAsync(string id) =>
            await Get(id);

        public async Task<TRepoEntity> CreateAsync(TRepoEntity item)
        {
            _logger.LogInformation($"Requested creation of {_repoEntityName}: {JsonConvert.SerializeObject(item)}");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.create").StartActive();
            try
            {
                // Assign unique id
                item.Id = ObjectId.GenerateNewId().ToString();
                var insertSpan = _tracer.BuildSpan("insertion").WithTag(Tags.DbType, "Mongo").AsChildOf(mainSpan.Span)
                    .Start();
                await Items.InsertOneAsync(item);
                insertSpan.Finish();
                await SendEvent();
                _logger.LogInformation($"Requested creation of {_repoEntityName} succeeded");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to create {_repoEntityName}");
                return default;
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }

        public async Task UpdateAsync(string id, TRepoEntity itemIn)
        {
            _logger.LogInformation(
                $"Requested update of {_repoEntityName} with id {id} with new value: {JsonConvert.SerializeObject(itemIn)}");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.update").StartActive();
            try
            {
                var replaceSpan = _tracer.BuildSpan("replace").WithTag(Tags.DbType, "Mongo").AsChildOf(mainSpan.Span)
                    .Start();
                await Items.ReplaceOneAsync(item => item.Id == id, itemIn);
                replaceSpan.Finish();
                await SendEvent();
                _logger.LogInformation($"Requested update of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to update {_repoEntityName}");
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }

        public async Task RemoveAsync(TRepoEntity itemIn)
        {
            _logger.LogInformation($"Requested delete of {_repoEntityName}: {JsonConvert.SerializeObject(itemIn)}");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.delete").StartActive();
            try
            {
                var deleteSpan = _tracer.BuildSpan("deletion").WithTag(Tags.DbType, "Mongo").AsChildOf(mainSpan.Span)
                    .Start();
                await Items.DeleteOneAsync(item => item.Id == itemIn.Id);
                deleteSpan.Finish();
                await SendEvent();
                _logger.LogInformation($"Requested delete of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete {_repoEntityName}");
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }

        public async Task RemoveAsync(string id)
        {
            _logger.LogInformation($"Requested delete of {_repoEntityName} with id {id}");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.delete").StartActive();
            try
            {
                var deleteSpan = _tracer.BuildSpan("deletion").WithTag(Tags.DbType, "Mongo").AsChildOf(mainSpan.Span)
                    .Start();
                await Items.DeleteOneAsync(item => item.Id == id);
                deleteSpan.Finish();
                await SendEvent();
                // TODO: On deletion no need fully recache - just remove hash entry!
                await SendEvent(eventType: EventType.Deleted, payload: id);
                _logger.LogInformation($"Requested delete of {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete {_repoEntityName}");
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }

        public async Task RemoveManyAsync(string[] ids)
        {
            _logger.LogInformation(
                $"Requested delete of multiple {_repoEntityName} with ids {JsonConvert.SerializeObject(ids)}");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.delete").StartActive();
            try
            {
                var deleteSpan = _tracer.BuildSpan("deletion").WithTag(Tags.DbType, "Mongo").AsChildOf(mainSpan.Span)
                    .Start();
                await Items.DeleteManyAsync(item => ids.Contains(item.Id));
                deleteSpan.Finish();
                await SendEvent();
                // TODO: On deletion no need fully recache - just remove hash entry!
                await SendEvent(eventType: EventType.Deleted, payload: ids);
                _logger.LogInformation($"Requested delete of multiple {_repoEntityName} succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to delete multiple {_repoEntityName}");
            }
            finally
            {
                mainSpan.Span.Finish();
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

        /// <summary>
        /// Get items from cache
        /// </summary>
        /// <param name="hashMode">Get from HashSet</param>
        /// <returns>Entries of entity</returns>
        private async Task<IEnumerable<TRepoEntity>> Get(bool hashMode = true)
        {
            _logger.LogInformation($"Requested all {_repoEntityName} items");
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.get-all").StartActive();
            try
            {
                var redisResult = hashMode
                    ? await _cacheService.HashGetAllAsync<TRepoEntity>(CommandFlags.None)
                    : await _cacheService.GetAsync<List<TRepoEntity>>(_repoEntityName);
                if (redisResult != default)
                {
                    var mongoRepoEntities = redisResult as TRepoEntity[] ?? redisResult.ToArray();
                    _logger.LogInformation(
                        $"Request of all {_repoEntityName} items returned {mongoRepoEntities.Length} items");
                    return mongoRepoEntities;
                }

                _logger.LogWarning($"Items of type {_repoEntityName} are not presented in cache");
                var dbSpan = _tracer.BuildSpan("mongo-cached-repo.get.db").WithTag(Tags.DbType, "Mongo")
                    .AsChildOf(mainSpan.Span).Start();
                var dbResult = (await Items.FindAsync(item => true)).ToEnumerable();
                dbSpan.Finish();
                await SendEvent();

                var repoEntities = dbResult as TRepoEntity[] ?? dbResult.ToArray();
                _logger.LogInformation($"Request of all {_repoEntityName} items returned {repoEntities.Length} items");
                return repoEntities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to return all {_repoEntityName} items");
                return default;
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <param name="id">Identifier of entry</param>
        /// <param name="hashMode">Get from HashSet</param>
        /// <returns>Entry of entity</returns>
        private async Task<TRepoEntity> Get(string id, bool hashMode = true)
        {
            var mainSpan = _tracer.BuildSpan("mongo-cached-repo.get").StartActive();
            _logger.LogInformation($"Requested {_repoEntityName} with id: {id}");
            try
            {
                var redisResult = hashMode
                    ? await _cacheService.HashGetAsync<TRepoEntity>(id, CommandFlags.None)
                    : (await _cacheService.GetAsync<List<TRepoEntity>>(_repoEntityName))?.FirstOrDefault(
                        x => x.Id == id);

                if (redisResult != null)
                {
                    _logger.LogInformation($"Request of {_repoEntityName} with id: {id} succeeded");
                    return redisResult;
                }

                _logger.LogWarning($"Item of type {_repoEntityName}  with id: {id} is not presented in cache");
                var dbSpan = _tracer.BuildSpan("mongo-cached-repo.get.db").WithTag(Tags.DbType, "Mongo")
                    .AsChildOf(mainSpan.Span).Start();
                var dbResult = await (await Items.FindAsync(item => item.Id == id)).FirstOrDefaultAsync();
                dbSpan.Finish();
                await SendEvent();
                _logger.LogInformation($"Request of {_repoEntityName} with id: {id} succeeded");
                return dbResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error during attempt to return all {_repoEntityName} items");
                return default;
            }
            finally
            {
                mainSpan.Span.Finish();
            }
        }
    }
}