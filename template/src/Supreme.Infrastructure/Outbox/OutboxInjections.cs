using Supreme.Domain.Exceptions;
using Supreme.Domain.Outbox;
using Supreme.Domain.Settings;
using Supreme.Infrastructure.Db;
using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Reflection;

namespace Supreme.Infrastructure.Outbox;

public static class OutboxInjections
{
    public static void AddOutboxInjections(this IServiceCollection services, IConfiguration configuration, 
        List<Assembly> assemblies)
    {
        services.Scan(s =>
            s.FromAssemblies(assemblies)
                .AddClasses(c => c.AssignableTo<ICapSubscribe>())
                .AsSelf()
                .WithTransientLifetime());

        var rabbitMqSetting = configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>() 
                              ?? throw new DevelopmentException("RabbitMqSetting is not configured properly!");
        
        services.AddSingleton(rabbitMqSetting);

        services.AddCap(opt =>
        {
            opt.GroupNamePrefix = "Supreme";
            opt.UseEntityFramework<SupremeContext>();
            opt.UseRabbitMQ(mqOptions =>
            {
                mqOptions.HostName = rabbitMqSetting.Host;
                mqOptions.Port = rabbitMqSetting.Port;
                mqOptions.VirtualHost = rabbitMqSetting.VirtualHost;
                mqOptions.UserName = rabbitMqSetting.Username;
                mqOptions.Password = rabbitMqSetting.Password;
                mqOptions.ExchangeName = "Supreme.default.router".ToLowerInvariant();
            });

            opt.FailedThresholdCallback = CapExtraConfiguration.FailedThresholdCallback;
            opt.FailedRetryCount = 5;
            opt.SucceedMessageExpiredAfter = 3 * 3600; // 3 hours
            opt.UseDashboard();
        });

        services.AddSingleton<IConnectionFactory>(provider =>
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMqSetting.Host,
                Port = rabbitMqSetting.Port,
                VirtualHost = rabbitMqSetting.VirtualHost,
                UserName = rabbitMqSetting.Username,
                Password = rabbitMqSetting.Password,
                AutomaticRecoveryEnabled = true
            };
            
            return connectionFactory;
        });
 
        services.AddScoped<IOutboxClient, OutboxClient>();
    }
}
