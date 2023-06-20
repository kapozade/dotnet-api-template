using EasyCaching.Core;
using Supreme.Domain.Cache;

namespace Supreme.Infrastructure.Cache;

public class HybridCacheService<T> : IHybridCacheService<T>
{
    private readonly IHybridCachingProvider _provider;

    public HybridCacheService(
        IHybridCachingProvider provider
        )
    {
        _provider = provider;
    }
    
    public async Task<T?> GetAsync(string key)
    {
        var result = await _provider.GetAsync<T>(key);
        return result.Value;
    }

    public async Task SetAsync(string key, T entry, int? expireInMinutes = null)
    {
        var expireIn = expireInMinutes ?? int.MaxValue;
        await _provider.SetAsync(key, entry, TimeSpan.FromMinutes(expireIn));
    }
}
