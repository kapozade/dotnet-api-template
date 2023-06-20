using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Supreme.Infrastructure.RateLimiting.Core;

namespace Supreme.Api.Filters.RateLimits;

public sealed class FixedWindowRateLimitFilterAttribute : ActionFilterAttribute
{
    private readonly string _policyKey;
    private readonly int _limit;
    private readonly int _expireInSeconds;

    public FixedWindowRateLimitFilterAttribute(
        string policyKey,
        int limit,
        int expireInSeconds
    )
    {
        _policyKey = policyKey;
        _limit = limit;
        _expireInSeconds = expireInSeconds;
    }
    
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var rateLimits = context.HttpContext.RequestServices.GetServices<IRateLimit>();
        var rateLimit = rateLimits.Single(x => x.RateLimitType == RateLimitTypes.FixedWindow);
        var rateLimitOptions = new RateLimitOptions
        {
            Limit = _limit, Period = TimeSpan.FromSeconds(_expireInSeconds), PolicyKey = _policyKey
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
