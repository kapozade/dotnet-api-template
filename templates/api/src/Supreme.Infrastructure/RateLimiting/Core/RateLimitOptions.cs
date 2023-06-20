namespace Supreme.Infrastructure.RateLimiting.Core;

public struct RateLimitOptions
{
    public string PolicyKey { get; set; }
    public int Limit { get; set; }
    public TimeSpan? Period { get; set; }
    public TimeSpan? ReplenishPeriod { get; set; }
    public int TokensPerPeriod { get; set; }
    public string? RequestId { get; set; }
}
