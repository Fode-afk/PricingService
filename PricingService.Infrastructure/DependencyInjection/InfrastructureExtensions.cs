using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using migApp.Shared.Behaviours;
using migApp.Shared.Caching;
using migApp.Shared.Domain.Primitives;
using migApp.Shared.Grpc;
using migApp.Shared.Utils;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using PricingService.Application.Interfaces.Data;
using PricingService.Application.Interfaces.Metrics;
using PricingService.Application.Interfaces.Services;
using PricingService.Infrastructure.Behaviours;
using PricingService.Infrastructure.Data;
using PricingService.Infrastructure.DomainEvents;
using PricingService.Infrastructure.Messaging;
using PricingService.Infrastructure.Messaging.Consumers;
using PricingService.Infrastructure.Messaging.IntegrationEvents;
using PricingService.Infrastructure.Observability;
using PricingService.Infrastructure.Services;
using PricingService.Infrastructure.Services.Grpc.Clients;
using RabbitMQ.Client;
using ZiggyCreatures.Caching.Fusion;
using Protos = CurrencyService.Api.Grpc.V1.Protos;

namespace PricingService.Infrastructure.DependencyInjection;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) => 
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddCache(configuration)
            .AddHealth(configuration)
            .AddGrpcServices(configuration)
            .AddCircuitBreaker()
            .AddMessaging()
            .AddIntegrationEventHandlers()
            .AddObservability(configuration)
            .AddBehaviours();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyService, CurrencyServiceClient>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {                 
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", Schemas.PricesWrite);
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
            .WithDistributedCache(sp => sp.GetRequiredService<IDistributedCache>())
            .WithStackExchangeRedisBackplane(o => o.Configuration = redisConnection)
            .WithSerializer(new JsonFusionCacheSerializer());

        return services;
    }

    private static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseConnectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is missing");

        var rabbitMq = configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>()
                ?? throw new InvalidOperationException("RabbitMQ configuration is missing.");

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is missing");

        services.AddGrpcHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"))
            .AddSqlServer(
                connectionString: databaseConnectionString,
                name: "mssql",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRabbitMQ(
                factory: async sp =>
                {
                    var factory = new ConnectionFactory()
                    {
                        HostName = rabbitMq.Host,
                        Port = rabbitMq.Port,
                        VirtualHost = rabbitMq.VirtualHost,
                        UserName = rabbitMq.Username,
                        Password = rabbitMq.Password
                    };
                    return await factory.CreateConnectionAsync();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"])
            .AddRedis(
                redisConnectionString: redisConnectionString,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"]);

        return services;
    }

    private static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        string CurrencyServiceConnectionString = configuration.GetConnectionString("CurrencyService")
            ?? throw new InvalidOperationException("CurrencyService connection string is missing");

        services.AddGrpcClient<Protos.CurrencyService.CurrencyServiceClient>(options =>
        {
            options.Address = new Uri(CurrencyServiceConnectionString);
        });

        return services;
    }

    private static IServiceCollection AddCircuitBreaker(this IServiceCollection services) =>
       services.AddSingleton(sp =>
           CircuitBreakerPolicy.GetCircuitBreakerPolicy(
               sp.GetRequiredService<ILogger<IAsyncPolicy>>()));

    private static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services
           .AddOptions<RabbitMqOptions>()
           .BindConfiguration(RabbitMqOptions.SectionName)
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Host),
               "RabbitMQ:Host is required.")
           .Validate(
               options => options.Port > 0,
               "RabbitMQ:Port must be greater than zero.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Username),
               "RabbitMQ:Username is required.")
           .Validate(
               options => !string.IsNullOrWhiteSpace(options.Password),
               "RabbitMQ:Password is required.")
           .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("pricing-service", false));

            x.AddConsumersFromNamespaceContaining<ConsumersAssemblyMarker>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context
                    .GetRequiredService<IOptions<RabbitMqOptions>>()
                    .Value;

                cfg.Host(options.Host,
                    options.Port,
                    options.VirtualHost,
                    host =>
                    {
                        host.Username(options.Username);
                        host.Password(options.Password);
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

    private static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var otlpEndpoint = configuration["Otlp:Endpoint"]
            ?? throw new InvalidOperationException("OtlpEndpoint is not configured");

        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("PricingService"))
            .UseOtlpExporter(OtlpExportProtocol.Grpc, new Uri(otlpEndpoint))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(opts =>
                    opts.Filter = ctx =>
                        !ctx.Request.Path.StartsWithSegments("/health"))
                .AddHttpClientInstrumentation()
                .AddFusionCacheInstrumentation()
                .AddSource("MassTransit")
                .AddSource("PricingService"))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddFusionCacheInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("MassTransit")
                .AddMeter(PricingServiceMetrics.MeterName));

        services.AddSingleton<IPricingMetrics, PricingServiceMetrics>();

        return services;
    }

    private static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
