using Supreme.Domain.Exceptions;
using Supreme.Infrastructure.RateLimiting.Core;
using StackExchange.Redis;
using System.Text;

namespace Supreme.Infrastructure.RateLimiting;

public sealed class SlidingWindowRateLimit : IRateLimit
{
    private readonly LuaScript _scriptToExecute;
    private readonly IDatabase _database;
    public RateLimitTypes RateLimitType { get => RateLimitTypes.SlidingWindow; }

    public SlidingWindowRateLimit(
            IDatabase database
        )
    {
        _database = database;
        _scriptToExecute = LuaScript.Prepare(
            @"
                local limit = tonumber(@permit_limit)
                local now = tonumber(@current_time)
                local window = tonumber(@window)

                redis.call('ZREMRANGEBYSCORE', @rate_limit_key, '-inf', now - window)

                local count = redis.call('ZCARD', @rate_limit_key)
                local allowed = count < limit

                if allowed
                then
                    redis.call('ZADD', @rate_limit_key, now, @request_id)
                end

                redis.call('EXPIREAT', @rate_limit_key, now + window + 1)
                local remaining = limit - count

                return { allowed, remaining }
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
        if (string.IsNullOrWhiteSpace(options.RequestId))
            throw new DevelopmentException("RequestId should not be null or empty");
        
        var keyBuilder = new StringBuilder("slidingwindow:{{").Append(options.PolicyKey).Append("}}");
        var result = (RedisValue[]?)await _database.ScriptEvaluateAsync(_scriptToExecute,
            new
            {
                permit_limit = (RedisValue)options.Limit,
                rate_limit_key = (RedisKey)keyBuilder.ToString(),
                current_time = (RedisValue)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                window = (RedisValue)options.Period.Value.TotalSeconds,
                request_id = (RedisValue)options.RequestId
            });
        
        var rateLimitState = new RateLimitState { Limit = options.Limit  };

        if (result != null)
        {
            rateLimitState.IsWithinLimit = (bool)result[0];
            rateLimitState.Remaining = (long)result[1];
        }

        return rateLimitState;
    }
}
