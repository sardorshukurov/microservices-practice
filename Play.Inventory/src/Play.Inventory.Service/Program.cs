using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);
const string allowedOriginSettings = "AllowedOrigin";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryitems")
    .AddMongoRepository<CatalogItem>("catalogitems")
    .AddMassTransitWithRabbitMq();

AddCatalogClient(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseCors(b =>
    {
        b.WithOrigins(app.Configuration[allowedOriginSettings]!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void AddCatalogClient(IServiceCollection services)
{
    Random random = new Random();
    services
        .AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7054");
        })
        .AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) 
                            + TimeSpan.FromMilliseconds(random.Next(0, 1000))
        ))
        .AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(15)
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}