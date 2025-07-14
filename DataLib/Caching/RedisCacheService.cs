using StackExchange.Redis;
using System.Text.Json;
using Serilog;

namespace DataLib.Caching
{
    public class RedisCacheService
    {
        private readonly IDatabase _cache;

        public RedisCacheService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            try
            {
                Log.Information("Connecting to Redis with connection string: {ConnectionString}", connectionString);
                var redis = ConnectionMultiplexer.Connect(connectionString);
                _cache = redis.GetDatabase();
                Log.Information("Successfully connected to Redis.");
            }
            catch (RedisConnectionException ex)
            {
                Log.Error(ex, "Failed to connect to Redis with connection string: {ConnectionString}. Exception: {Message}, StackTrace: {StackTrace}", connectionString, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            try
            {
                Log.Information("Setting cache for key: {Key}, expiration: {Expiration}", key, expiration);
                var json = JsonSerializer.Serialize(value);
                await _cache.StringSetAsync(key, json, expiration);
                Log.Information("Cache set successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to set cache for key: {Key}. Exception: {Message}, StackTrace: {StackTrace}", key, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                Log.Information("Retrieving cache for key: {Key}", key);
                var json = await _cache.StringGetAsync(key);
                if (json.HasValue)
                {
                    var result = JsonSerializer.Deserialize<T>(json);
                    Log.Information("Cache retrieved successfully for key: {Key}", key);
                    return result;
                }
                Log.Information("No cache found for key: {Key}", key);
                return default;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve cache for key: {Key}. Exception: {Message}, StackTrace: {StackTrace}", key, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            try
            {
                Log.Information("Removing cache for key: {Key}", key);
                await _cache.KeyDeleteAsync(key);
                Log.Information("Cache removed successfully for key: {Key}", key);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to remove cache for key: {Key}. Exception: {Message}, StackTrace: {StackTrace}", key, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}