using Microsoft.Extensions.DependencyInjection;
using Supreme.Infrastructure.RateLimiting.Core;

namespace Supreme.Infrastructure.RateLimiting;

public static class RateLimitingInjections
{
    public static void AddRateLimitInjections(this IServiceCollection services)
    {
        var rateLimits = typeof(CompositionRoot).Assembly
            .GetTypes()
            .Where(x =>
                typeof(IRateLimit).IsAssignableFrom(x)
                && x is { IsAbstract: false, IsInterface: false });
        
        foreach (var rateLimit in rateLimits) 
            services.AddSingleton(typeof(IRateLimit), rateLimit);
    }
}
