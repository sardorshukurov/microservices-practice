using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryitems");

Random jitterer = new Random();

builder.Services
    .AddHttpClient<CatalogClient>(client =>
    {
        client.BaseAddress = new Uri("https://localhost:7054");
    })
    .AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().WaitAndRetryAsync(
        5,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) 
                        + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000))
    ))
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();