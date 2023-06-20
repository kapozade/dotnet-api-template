using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Supreme.Infrastructure.RateLimiting.Core;

namespace Supreme.Api.Filters.RateLimits;

public sealed class ConcurrentRateLimitFilterAttribute : ActionFilterAttribute
{
    private readonly string _policyKey;
    private readonly int _limit;
    private readonly int _expireInSeconds;

    public ConcurrentRateLimitFilterAttribute(
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
        var rateLimit = rateLimits.Single(x => x.RateLimitType == RateLimitTypes.Concurrent);
        var rateLimitOptions = new RateLimitOptions
        {
            Limit = _limit, PolicyKey = _policyKey, Period = TimeSpan.FromSeconds(_expireInSeconds)
        };
        var rateLimitState = await rateLimit.GetCurrentLimitStateAsync(rateLimitOptions);
        if (!rateLimitState.IsWithinLimit)
        {
            context.HttpContext.Response.Headers.Add("X-RateLimit-Limit" , rateLimitState.Limit.ToString());
            context.HttpContext.Response.Headers.Add("X-RateLimit-Remaining" , rateLimitState.Remaining.ToString());
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new StatusCodeResult(StatusCodes.Status429TooManyRequests);

            return;
        }

        await next();    
    }
}
