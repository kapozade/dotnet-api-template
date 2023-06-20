namespace Supreme.Infrastructure.RateLimiting.Core;

public enum RateLimitTypes
{
    FixedWindow = 1,
    Concurrent = 2,
    SlidingWindow = 3,
    TokenBucket = 4
}
