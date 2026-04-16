using PricingService.Application.DependencyInjection;
using PricingService.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseHttpsRedirection();

//app.MapGrpcService<GrpcServer>();
app.MapGrpcHealthChecksService();

app.Run();