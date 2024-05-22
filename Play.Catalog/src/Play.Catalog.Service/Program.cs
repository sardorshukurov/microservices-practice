using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;

var builder = WebApplication.CreateBuilder(args);
const string allowedOriginSettings = "AllowedOrigin";

builder.Services
    .AddMongo()
    .AddMongoRepository<Item>("items")
    .AddMassTransitWithRabbitMq();

builder.Services.AddControllers(o =>
{
    o.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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