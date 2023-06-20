using EasyCaching.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supreme.Domain.Cache;
using Supreme.Domain.Exceptions;
using Supreme.Domain.Extensions;
using Supreme.Domain.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Supreme.Infrastructure.Cache;

public static class CacheInjections
{
    public static void AddCacheInjections(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var redisSettings = configuration.GetSection("RedisSettings").Get<RedisSettings>() 
                            ?? throw new DevelopmentException("RedisSettings is null.");

        void EasyCacheConfig(JsonSerializerSettings x)
        {
            x.ContractResolver = new CamelCasePropertyNamesContractResolver();
            x.DefaultValueHandling = DefaultValueHandling.Include;
            x.StringEscapeHandling = StringEscapeHandling.Default;
            x.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
            x.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            x.TypeNameHandling = TypeNameHandling.None;
            x.Converters.Add(new StringEnumConverter());
        }

        serviceCollection.AddSingleton<IDatabase>(provider =>
        {
            var redis = ConnectionMultiplexer.Connect(redisSettings.GenerateConnectionString());
            return redis.GetDatabase();
        });

        serviceCollection.AddEasyCaching(opt =>
        {
            opt.WithJson(EasyCacheConfig, "default-serializer");
            opt.UseInMemory("in-memory-provider");
            opt.UseRedis(cfg =>
            {
                cfg.DBConfig.Endpoints.Add(new ServerEndPoint(redisSettings.Host, redisSettings.Port));
                cfg.DBConfig.Database = redisSettings.DbId;
                cfg.DBConfig.Username = redisSettings.User;
                cfg.DBConfig.Password = redisSettings.Password;
                cfg.SerializerName = "default-serializer";
            }, "distributed-cache-provider");
            opt.UseHybrid(cfg =>
            {
                cfg.TopicName = "redis-bus-topic";
                cfg.LocalCacheProviderName = "in-memory-provider";
                cfg.DistributedCacheProviderName = "distributed-cache-provider";
                cfg.EnableLogging = false;
            }).WithRedisBus(bus =>
            {
                bus.Endpoints.Add(new ServerEndPoint(redisSettings.Host, redisSettings.Port));
                bus.SerializerName = "default-serializer";
            });
        });

        serviceCollection.AddScoped(typeof(IHybridCacheService<>), typeof(HybridCacheService<>));
        serviceCollection.AddScoped(typeof(IRedisCacheService<>), typeof(RedisCacheService<>));
        serviceCollection.AddScoped(typeof(IMemoryCacheService<>), typeof(MemoryCacheService<>));
    }
}
