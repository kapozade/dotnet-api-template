using Supreme.Domain.Exceptions;
using Supreme.Infrastructure.RateLimiting.Core;
using StackExchange.Redis;
using System.Text;

namespace Supreme.Infrastructure.RateLimiting;

public sealed class FixedWindowRateLimit : IRateLimit
{
    private readonly LuaScript _scriptToExecute;
    private readonly IDatabase _database;
    public RateLimitTypes RateLimitType { get => RateLimitTypes.FixedWindow; }

    public FixedWindowRateLimit(
            IDatabase database
        )
    {
        _database = database;
        _scriptToExecute = LuaScript.Prepare(
            @"
                local limit = tonumber(@permit_limit)
                local now = tonumber(@current_time)
                local ttl = tonumber(@time_to_live)

                local count = redis.call('INCR', @rate_limit_key)

                if count == 1 then
                    redis.call('EXPIRE', @rate_limit_key, ttl)
                end

                local remaining = redis.call('TTL', @rate_limit_key)                

                if count <= limit then
                    return { true, limit - count, remaining }
                else
                    return { false, 0, remaining }
                end
            ");
    }
    
    public async Task<RateLimitState> GetCurrentLimitStateAsync(RateLimitOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.PolicyKey))
            throw new DevelopmentException("PolicyKey should not be null or empty");
        if (options.Limit <= 0)
            throw new DevelopmentException("Limit should be greater than 1");
        if (!options.Period.HasValue || options.Period <= TimeSpan.Zero)
            throw new DevelopmentException("Invalid period");
        
        var keyBuilder = new StringBuilder("fixedwindow:{{").Append(options.PolicyKey).Append("}}");
        var result = (RedisValue[]?)await _database.ScriptEvaluateAsync(_scriptToExecute,
            new
            {
                permit_limit = (RedisValue)options.Limit,
                rate_limit_key = (RedisKey)keyBuilder.ToString(),
                current_time = (RedisValue)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                time_to_live = (RedisValue)options.Period.Value.TotalSeconds
            });
        
        var rateLimitState = new RateLimitState { Limit = options.Limit  };

        if (result != null)
        {
            rateLimitState.IsWithinLimit = (bool)result[0];
            rateLimitState.Remaining = (long)result[1];
            rateLimitState.Reset = TimeSpan.FromSeconds((long)result[2]);
        }

        return rateLimitState;
    }
}
