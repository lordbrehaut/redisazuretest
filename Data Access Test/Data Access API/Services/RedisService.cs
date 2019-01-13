using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using StackExchange.Redis;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataAccessAPI.Services
{
    public class RedisService: IRedisService
    {
        private readonly IDatabase cache;

        public RedisService(CacheSettings cacheSettings)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                string cacheConnection = cacheSettings.CacheConnectionString;
                return ConnectionMultiplexer.Connect(cacheConnection);
            });

            cache = lazyConnection.Value.GetDatabase();
        }

        public virtual async Task<(string, long)> GetFromCache(string key)
        {
            return (await cache.StringGetAsync(key), 0);
        }

        public virtual async Task<long> SetInCache(string key, string value)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await cache.StringSetAsync(key, value);
            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }
    }
}
