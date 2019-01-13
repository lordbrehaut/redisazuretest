using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessAPI.Services.Interfaces;
using DataAccessAPI.Settings;

namespace DataAccessAPI.Services
{
    public class SignedRedisService : RedisService, ISignedRedisService
    {
        private readonly ISignedService _signedService;

        public SignedRedisService(ISignedService signedService, CacheSettings cacheSettings) : base(cacheSettings)
        {
            _signedService = signedService;
        }

        public override async Task<(string, long)> GetFromCache(string key)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await base.GetFromCache(key);
            stopwatch.Stop();
            var resultArray = Convert.FromBase64String(result.Item1);
            if(_signedService.Verify(resultArray, out int hashLength))
            {
                var newArray = new byte[resultArray.Length - hashLength];
                Buffer.BlockCopy(resultArray, 0, newArray, 0, newArray.Length);
                return (Encoding.UTF8.GetString(newArray), stopwatch.ElapsedTicks);
            }
            return (string.Empty, 0);
        }

        public override async Task<long> SetInCache(string key, string value)
        {
            var content = Encoding.UTF8.GetBytes(value);
            string signedContent;
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    var hash = _signedService.GenerateHash(content);
                    binaryWriter.Write(content);
                    binaryWriter.Write(hash);
                }
                signedContent = Convert.ToBase64String(memoryStream.ToArray());
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await base.SetInCache(key, signedContent);
            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }
    }
}