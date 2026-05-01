using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using migApp.Shared.Caching;
using migApp.Shared.Grpc;
using migApp.Shared.Utils;
using Polly;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Services;
using PricingService.Domain.Primitives;
using PricingService.Infrastructure.Data;
using PricingService.Infrastructure.Data.Repositories;
using PricingService.Infrastructure.DependencyInjection;
using PricingService.Infrastructure.DomainEvents;
using PricingService.Infrastructure.Messaging.Consumers;
using PricingService.Infrastructure.Messaging.IntegrationEvents;
using PricingService.Infrastructure.Services;
using PricingService.Infrastructure.Services.Grpc.Clients;
using RabbitMQ.Client;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using Protos = CurrencyService.Api.Grpc.V1.Protos;

namespace PricingService.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => 
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddCache(configuration)
            .AddHealthChecks(configuration)
            .AddGrpc(configuration)
            .AddCircuitBreaker()
            .AddMassTransit(configuration)
            .AddIntegrationEventHandlers();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyService, CurrencyServiceClient>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        services.AddScoped<IMoneyConverter, MoneyConverter>();

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IPriceReadRepository, PriceReadRepository>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {                 
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.Prices);
            }));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {       
        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is missing");
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;       
            options.InstanceName = "PricingService:";
        });

        services
            .AddFusionCache()
            .WithOptions(options =>
            {
                options.DefaultEntryOptions = new FusionCacheEntryOptions
                {
                    Duration = TimeSpan.FromMinutes(5),

                    IsFailSafeEnabled = false,

                    AllowBackgroundDistributedCacheOperations = true,
                    AllowBackgroundBackplaneOperations = true,

                    SkipBackplaneNotifications = false,
                    JitterMaxDuration = TimeSpan.Zero,

                    FactorySoftTimeout = TimeSpan.FromMilliseconds(300),
                    FactoryHardTimeout = TimeSpan.FromSeconds(3)
                };
            })
            .WithDistributedCache(sp =>
                sp.GetRequiredService<IDistributedCache>())
            .WithBackplane(sp => new RedisBackplane(
                new RedisBackplaneOptions
                {
                    Configuration = redisConnection
                }))
            .WithSerializer(new JsonFusionCacheSerializer())
            .TryWithAutoSetup();

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("Database")!,
                name: "mssql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRabbitMQ(
                factory: sp =>
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["RabbitMQ:Host"]!,
                        Port = int.Parse(configuration["RabbitMQ:Port"]!)
                    };
                    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services.AddGrpcClient<Protos.CurrencyService.CurrencyServiceClient>(options =>
        {
            options.Address = new Uri(configuration.GetConnectionString("CurrencyService")!);
        });

        return services;
    }

    private static IServiceCollection AddCircuitBreaker(this IServiceCollection services) =>
       services.AddSingleton(sp =>
           CircuitBreakerPolicy.GetCircuitBreakerPolicy(
               sp.GetRequiredService<ILogger<IAsyncPolicy>>()
           )
       );

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumersFromNamespaceContaining<ConsumersAssemblyMarker>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"]!, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(context);
            });     
        });

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        var assembly = typeof(IntegrationEventsAssemblyMarker).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IPreCommitDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
