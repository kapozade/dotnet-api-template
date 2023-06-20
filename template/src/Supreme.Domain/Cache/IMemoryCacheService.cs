namespace Supreme.Domain.Cache;

public interface IMemoryCacheService<T>
{
    T? Get(string key);
    void Set(string key, T entry, int? expireInMinutes = null);
}
