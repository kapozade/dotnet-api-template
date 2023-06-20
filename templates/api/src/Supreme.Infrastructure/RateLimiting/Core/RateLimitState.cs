namespace Supreme.Infrastructure.RateLimiting.Core;

public sealed record RateLimitState
{
    public bool IsWithinLimit { get; set; }
    public long Limit { get; init; }
    public long Remaining { get; set; }
    public TimeSpan? Reset { get; set; }
}
