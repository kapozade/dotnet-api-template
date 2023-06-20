using EasyCaching.Core;
using Supreme.Domain.Cache;
using Newtonsoft.Json;

namespace Supreme.Infrastructure.Cache;

public class RedisCacheService<T> : IRedisCacheService<T>
{
    private readonly IRedisCachingProvider _provider;
    
    public RedisCacheService(
        IEasyCachingProviderFactory providerFactory
        )
    {
        _provider = providerFactory.GetRedisProvider("distributed-cache-provider");
    }

    public async Task<T?> GetAsync(string key)
    {
        var cachedValue = await _provider.StringGetAsync(key);
        return string.IsNullOrWhiteSpace(cachedValue)
            ? default
            : JsonConvert.DeserializeObject<T>(cachedValue);
    }

    public async Task SetAsync(string key, T entry, int? expireInMinutes = null)
    {
        var expireIn = expireInMinutes ?? int.MaxValue;
        await _provider.StringSetAsync(key, JsonConvert.SerializeObject(entry), TimeSpan.FromMinutes(expireIn));
    }
}
