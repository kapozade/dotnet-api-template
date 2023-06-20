using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Supreme.Infrastructure.RateLimiting.Core;

namespace Supreme.Api.Filters.RateLimits;

public sealed class TokenBucketRateLimitFilterAttribute : ActionFilterAttribute 
{
    private readonly string _policyKey;
    private readonly int _limit;
    private readonly int _tokensPerPeriod;
    private readonly int _replenishPeriodInSeconds;

    public TokenBucketRateLimitFilterAttribute(
        string policyKey,
        int limit,
        int tokensPerPeriod,
        int replenishPeriodInSeconds
        )
    {
        _policyKey = policyKey;
        _limit = limit;
        _tokensPerPeriod = tokensPerPeriod;
        _replenishPeriodInSeconds = replenishPeriodInSeconds;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rateLimits = context.HttpContext.RequestServices.GetServices<IRateLimit>();
        var rateLimit = rateLimits.Single(x => x.RateLimitType == RateLimitTypes.TokenBucket);
        var rateLimitOptions = new RateLimitOptions
        {
            Limit = _limit, 
            TokensPerPeriod = _tokensPerPeriod,
            PolicyKey = _policyKey,
            ReplenishPeriod = TimeSpan.FromSeconds(_replenishPeriodInSeconds)
        };

        var rateLimitState = await rateLimit.GetCurrentLimitStateAsync(rateLimitOptions);
        if (!rateLimitState.IsWithinLimit)
        {
            context.HttpContext.Response.Headers.Add("X-RateLimit-Limit" , rateLimitState.Limit.ToString());
            context.HttpContext.Response.Headers.Add("X-RateLimit-Remaining" , rateLimitState.Remaining.ToString());
            if (rateLimitState.Reset.HasValue)
            {
                var reset = DateTime.UtcNow.Add(rateLimitState.Reset.Value);
                context.HttpContext.Response.Headers.Add("X-RateLimit-Reset" , reset.ToString("O"));
                context.HttpContext.Response.Headers.Add("Retry-After" , reset.ToString("O"));
            }
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);

            return;
        }

        await next();
    }
}
