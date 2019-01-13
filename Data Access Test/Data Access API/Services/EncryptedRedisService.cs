using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataAccessAPI.Services
{
    public class EncryptedRedisService : RedisService, IEncryptedRedisService
    {
        private readonly IEncryptionService _encryptionService;

        public EncryptedRedisService(IEncryptionService encryptionService, CacheSettings cacheSettings) : base(cacheSettings)
        {
            _encryptionService = encryptionService;
        }

        public override async Task<(string, long)> GetFromCache(string key)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cipherText = await base.GetFromCache(key);
            stopwatch.Stop();
            return (_encryptionService.Decrypt(cipherText.Item1), stopwatch.ElapsedTicks);
        }

        public override async Task<long> SetInCache(string key, string value)
        {
            var cipherText = _encryptionService.Encrypt(value);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await base.SetInCache(key, cipherText);
            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }
    }
}