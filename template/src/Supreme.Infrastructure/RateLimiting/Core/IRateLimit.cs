namespace Supreme.Infrastructure.RateLimiting.Core;

public interface IRateLimit
{
    RateLimitTypes RateLimitType { get; }
    Task<RateLimitState> GetCurrentLimitStateAsync(RateLimitOptions options);
}
