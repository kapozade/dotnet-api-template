using EasyCaching.InMemory;
using Supreme.Domain.Cache;

namespace Supreme.Infrastructure.Cache;

public class MemoryCacheService<T> : IMemoryCacheService<T>
{
    private readonly IInMemoryCaching _inMemoryCaching;

    public MemoryCacheService(
        IInMemoryCaching inMemoryCaching
        )
    {
        _inMemoryCaching = inMemoryCaching;
    }
    
    public T? Get(string key)
    {
        return _inMemoryCaching.Get<T?>(key).Value;
    }

    public void Set(string key, T entry, int? expireInMinutes = null)
    {
        var expireIn = expireInMinutes ?? int.MaxValue;
        _inMemoryCaching.Set(key, entry, TimeSpan.FromMinutes(expireIn));
    }
}
