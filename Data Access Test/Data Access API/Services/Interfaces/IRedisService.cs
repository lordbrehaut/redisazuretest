using System.Threading.Tasks;

namespace DataAccessAPI.Services.Interfaces
{
    public interface IRedisService
    {
        Task<(string, long)> GetFromCache(string key);
        Task<long> SetInCache(string key, string value);
    }
}