using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.DataAccess.Extensions;
using Catalog.DataAccess.Interfaces;
using GreenPipes.Internals.Extensions;
using Newtonsoft.Json;
using OpenTracing;
using OpenTracing.Tag;
using StackExchange.Redis;

namespace Catalog.DataAccess.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly ITracer _tracer;
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static readonly object Locker = new();
        private static ConnectionMultiplexer Connection => _lazyConnection.Value;
        
        public RedisCacheService(IRedisOptions options, ITracer tracer)
        {
            _tracer = tracer;
            lock (Locker)
            {
                _lazyConnection ??=
                    new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.ConnectionString));
            }
        }

        public async Task<bool> SetRawAsync(RedisKey key, RedisValue value, TimeSpan? expiry,
            CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.set-raw").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var result = await Connection.GetDatabase().StringSetAsync(key, value, expiry, When.Always, flags);
            span.Finish();
            return result;
        }

        public async Task<bool> SetAsync<T>(RedisKey key, T value, TimeSpan? expiry, CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.set").AsChildOf(_tracer.ActiveSpan).Start();
            var serializeSpan = _tracer.BuildSpan("serialize").AsChildOf(span).Start();
            var jsonValue = JsonConvert.SerializeObject(value);
            serializeSpan.Finish();
            var result = await SetRawAsync(key, jsonValue, expiry, flags);
            span.Finish();
            return result;
        }

        public async Task<RedisValue> GetRawAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var span = _tracer.BuildSpan("redis-cache-service.get-raw").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var result = await Connection.GetDatabase().StringGetAsync(key, flags);
            span.Finish();
            return result;
        }

        public async Task<T> GetAsync<T>(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var span = _tracer.BuildSpan("redis-cache-service.get").AsChildOf(_tracer.ActiveSpan).Start();
            var result = await Connection.GetDatabase().StringGetAsync(key, flags);
            var deserializeSpan = _tracer.BuildSpan("deserialize").AsChildOf(span).Start();
            var retVal = result == RedisValue.Null ? default : JsonConvert.DeserializeObject<T>(result);
            deserializeSpan.Finish();
            span.Finish();
            return retVal;
        }
        
        public async Task<bool> RemoveAsync(RedisKey key, CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.del").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var result = await Connection.GetDatabase().KeyDeleteAsync(key, flags);
            span.Finish();
            return result;
        }

        public async Task HashSetAsync<T>(IEnumerable<T> value,
            CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.hset").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var conversionSpan = _tracer.BuildSpan("conversion").AsChildOf(span).Start();
            var hashValue = value.ConvertToHashEntryList();
            var key = HashSetExtensions.GetHashKey<T>();
            conversionSpan.Finish();
            await Connection.GetDatabase().HashSetAsync(key, hashValue.ToArray(), flags);
            span.Finish();
        }

        public async Task<IEnumerable<T>> HashGetAllAsync<T>(CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.hgetall").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var key = HashSetExtensions.GetHashKey<T>();
            var entries = await Connection.GetDatabase().HashGetAllAsync(key, flags);
            var conversionSpan = _tracer.BuildSpan("conversion").AsChildOf(span).Start();
            var result = entries.ConvertToCollection<T>();
            conversionSpan.Finish();
            span.Finish();
            return result;
        }

        public async Task<T> HashGetAsync<T>(RedisValue key, CommandFlags flags = CommandFlags.None)
        {
            var span = _tracer.BuildSpan("redis-cache-service.hget").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var entityKey = HashSetExtensions.GetHashKey<T>();
            var entry = await Connection.GetDatabase().HashGetAsync(entityKey, key, flags);
            var deserializeSpan = _tracer.BuildSpan("deserialize").AsChildOf(span).Start();
            var result = JsonConvert.DeserializeObject<T>(entry);
            deserializeSpan.Finish();
            span.Finish();
            return result;
        }

        public async Task<bool> HashAppendAsync<T>(T value, CommandFlags flags = CommandFlags.FireAndForget)
        {
            var span = _tracer.BuildSpan("redis-cache-service.hset-append").WithTag(Tags.DbType, "Redis")
                .AsChildOf(_tracer.ActiveSpan).Start();
            var entityKey = HashSetExtensions.GetHashKey<T>();
            var entryKey = value.GetHashEntryKey();
            var serializeSpan = _tracer.BuildSpan("serialize").AsChildOf(span).Start();
            var hashValue = JsonConvert.SerializeObject(value);
            serializeSpan.Finish();
            var result = await Connection.GetDatabase().HashSetAsync(entityKey, entryKey, hashValue);
            span.Finish();
            return result;
        }
    }
}