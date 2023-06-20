namespace Supreme.Domain.Cache;

public interface IRedisCacheService<T>
{
    Task<T?> GetAsync(string key);
    Task SetAsync(string key, T entry, int? expireInMinutes = null);
}
