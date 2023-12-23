using Supreme.Domain.Exceptions;
using Supreme.Domain.Settings;
using Supreme.Infrastructure.Db;
using Supreme.Infrastructure.PipelineBehaviors;
using Supreme.Infrastructure.Cache;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
#if (enable-outbox-pattern)
using Supreme.Infrastructure.Outbox;
#endif
#if (enable-rate-limiting)
using Supreme.Infrastructure.RateLimiting;
#endif
#if (enable-open-telemetry)
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
#endif


namespace Supreme.Infrastructure;

public static class CompositionRoot
{
    public static void Register(IServiceCollection serviceCollection, IConfiguration configuration,
        List<Assembly> assemblies)
    {
        // var applicationSetting = configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();
        //             ?? throw new DevelopmentException("ApplicationSettings is null");
        // serviceCollection.AddSingleton(applicationSetting);

        var connectionString = configuration.GetConnectionString("DatabaseConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new DevelopmentException("DatabaseConnection is null or empty!");

        serviceCollection.AddRepositoryInjections(configuration);
        serviceCollection.AddValidatorsFromAssemblies(assemblies);
        serviceCollection.AddExternalLibraryDependencies(configuration, assemblies);
        serviceCollection.AddCacheInjections(configuration);

#if (enable-outbox-pattern)
        serviceCollection.AddOutboxInjections(configuration, assemblies);
#endif
#if (enable-rate-limiting)
        serviceCollection.AddRateLimitInjections();
#endif
        serviceCollection.AddHealthChecks()
            .AddCheck<InfrastructureHealthCheck>("health");
    }
    
    private static void AddExternalLibraryDependencies(this IServiceCollection serviceCollection,
        IConfiguration configuration, List<Assembly> assemblies)
    {
        serviceCollection.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblies(assemblies.ToArray());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
#if (enable-outbox-pattern)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionalRequestPipelineBehavior<,>));
#endif
        });
#if (enable-open-telemetry)
        var jaegerSettings = configuration.GetSection("JaegerSettings").Get<JaegerSettings>()
                        ?? throw new DevelopmentException("JaegerSettings is null.");

        var resourceBuilder = ResourceBuilder.CreateDefault().AddService("Supreme");
        serviceCollection.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
                tracerProviderBuilder
                    .AddSource("Supreme")
                    .SetResourceBuilder(resourceBuilder.AddTelemetrySdk())
                    .AddAspNetCoreInstrumentation(
                        opt =>
                            opt.Filter = (req) =>
                                !req.Request.Path.ToUriComponent()
                                    .Contains("index.html", StringComparison.OrdinalIgnoreCase)
                                && !req.Request.Path.ToUriComponent()
                                    .Contains("swagger", StringComparison.OrdinalIgnoreCase))
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(opt =>
                    {
                        opt.SetDbStatementForText = true;
                        opt.RecordException = true;
                    })
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri(jaegerSettings.Url);
                    })
            );

        serviceCollection.Configure<AspNetCoreTraceInstrumentationOptions>(opt =>
        {
            opt.RecordException = true;
        });
#endif
    }
}
