using Supreme.Domain.Exceptions;
using Supreme.Infrastructure.RateLimiting.Core;
using StackExchange.Redis;
using System.Text;

namespace Supreme.Infrastructure.RateLimiting;

public sealed class TokenBucketRateLimit : IRateLimit
{
    private readonly LuaScript _scriptToExecute;
    private readonly IDatabase _database;
    public RateLimitTypes RateLimitType { get => RateLimitTypes.TokenBucket; }

    public TokenBucketRateLimit(
        IDatabase database
    )
    {
        _database = database;
        _scriptToExecute = LuaScript.Prepare(
            @"
            local limit = tonumber(@token_limit)
            local rate = tonumber(@tokens_per_period)
            local period = tonumber(@replenish_period)
            local requested = tonumber(@permit_count)

            redis.replicate_commands()
            
            local time = redis.call('TIME')
            local now = math.floor((time[1] * 1000) + (time[2] / 1000))
            local state = redis.call('MGET', @rate_limit_key, @timestamp_key)
            local remaining = tonumber(state[1]) or limit
            local last_refreshed = tonumber(state[2]) or 0
            local time_since_last_refreshed = math.max(0, now - last_refreshed)
            local period_since_last_refreshed = math.floor(time_since_last_refreshed / period)

            remaining = math.min(limit, remaining + (period_since_last_refreshed * rate))
            local last_replenishment_time = now
            if last_refreshed > 0 then
                last_replenishment_time = last_refreshed + (period_since_last_refreshed * period)
            end

            local is_within_limit = remaining >= requested
            if is_within_limit then
                remaining = remaining - requested
            end
            local periods_until_full = math.ceil(limit / rate)
            local ttl = math.ceil(periods_until_full * period)
            if is_within_limit then
                redis.call('SET', @rate_limit_key, remaining, 'PX', ttl)
                redis.call('SET', @timestamp_key, last_replenishment_time, 'PX', ttl)
            end
            local retry_after = 0
            if not is_within_limit then
                retry_after = period - now + last_replenishment_time
            end

            return { is_within_limit, remaining, retry_after }
            ");
    }

    public async Task<RateLimitState> GetCurrentLimitStateAsync(RateLimitOptions options){
        if (string.IsNullOrWhiteSpace(options.PolicyKey))
            throw new DevelopmentException("PolicyKey should not be null or empty");
        if (options.Limit <= 0)
            throw new DevelopmentException("Limit should be greater than 1");
        if (options.TokensPerPeriod <= 0)
            throw new DevelopmentException("TokensPerPeriod should be greater than 0");
        if (!options.ReplenishPeriod.HasValue || options.ReplenishPeriod <= TimeSpan.Zero)
            throw new DevelopmentException("Invalid ReplenishPeriod");

        var policyKey = new StringBuilder("tokenbucket:").Append(options.PolicyKey);
        var timestampKey = new StringBuilder("tokenbucket:").Append(options.PolicyKey).Append(":timestamp");

        var result = (RedisValue[]?)await _database.ScriptEvaluateAsync(
            _scriptToExecute,
            new
            {
                rate_limit_key = (RedisKey)policyKey.ToString(),
                timestamp_key = (RedisKey)timestampKey.ToString(),
                tokens_per_period = (RedisValue)options.TokensPerPeriod,
                token_limit = (RedisValue)options.Limit,
                replenish_period = (RedisValue)options.ReplenishPeriod.Value.TotalMilliseconds,
                permit_count = (RedisValue)1
            });
        
        var rateLimitState = new RateLimitState { Limit = options.Limit };
        if (result != null)
        {
            rateLimitState.IsWithinLimit = (bool)result[0];
            rateLimitState.Remaining = (long)result[1];
            rateLimitState.Reset = TimeSpan.FromSeconds((int)Math.Ceiling((decimal)result[2] / 1000));
        }
        return rateLimitState;
    }
}
