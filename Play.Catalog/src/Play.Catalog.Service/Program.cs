using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(o =>
{
    o.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMongo()
    .AddMongoRepository<Item>("items");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();