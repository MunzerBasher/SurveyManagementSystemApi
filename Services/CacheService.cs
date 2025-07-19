using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace SurveyManagementSystemApi.Services
{
    public class CacheService(IDistributedCache distributedCache) : ICacheService
    {
        private readonly IDistributedCache _distributedCache = distributedCache;


        public async Task<T?> GetTAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            var cachevalue = await _distributedCache.GetAsync(key, cancellationToken);
            return cachevalue is null ? null : JsonSerializer.Deserialize<T>(cachevalue);

        }

        public  async Task SetAsync<T>(string key,T value ,CancellationToken cancellationToken = default) where T : class
        {
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), cancellationToken);
        }


        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }



    }
}
