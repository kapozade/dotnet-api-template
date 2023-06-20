namespace Supreme.Domain.Cache;

public interface IHybridCacheService<T>
{
    Task<T?> GetAsync(string key);
    Task SetAsync(string key, T entry, int? expireInMinutes = null);
}
