using AzisFood.CacheService.Redis.Extensions;
using AzisFood.DataEngine.Cache.CacheService.Extensions;
using AzisFood.DataEngine.MQ.Rabbit.Extensions;
using AzisFood.DataEngine.Postgres.Extensions;
using AzisFood.MQ.Rabbit.Extensions;
using Catalog.Core;
using Catalog.DataAccess;
using Catalog.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Register mappings
builder.Services.AddMapper();

// Add RabbitMQ MassTransit
builder.Services.AddRabbitMQSupport(builder.Configuration);

// Add Redis config
builder.Services.AddRedisSupport(builder.Configuration);

// Add open tracing with Jaeger
builder.Services.AddJaeger(builder.Configuration);

// Add grpc services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Add database services
builder.Services
    .AddPostgresSupport(builder.Configuration)
    .AddPostgresContext<CatalogDbContext>()
    .UseCacheServiceAdapter()
    .UseRabbitCacheEventHandler();

// Add data seeder
builder.Services.AddTransient<CatalogDbSeeder>();

// Add core function services
builder.Services.AddCoreServices();

// Configure kestrel
builder.WebHost.ConfigureKestrel(configureOptions =>
{
    configureOptions.ListenAnyIP(5000, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapGrpcReflectionService();

app.MapGrpcService<CategoryService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

// Seed
app.Services.AddSeeder(app.Configuration);

app.Run();