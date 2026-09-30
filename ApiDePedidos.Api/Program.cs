using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Application.Orders.Commands;
using ApiDePedidos.Infrastructure.Persistence.Mongo;
using ApiDePedidos.Infrastructure.Persistence.Sql;
using Microsoft.OpenApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(
        typeof(CreateOrderCommand).Assembly));

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Orders")
        ?? throw new InvalidOperationException(
            "Connection string 'Orders' não configurada.")));

builder.Services.AddScoped<IOrderWriteRepository, SqlOrderWriteRepository>();
builder.Services.AddScoped<IOrderReferenceData, SqlOrderReferenceData>();

var mongoConnection = builder.Configuration
                          .GetConnectionString("OrdersMongo")
                      ?? throw new InvalidOperationException(
                          "Connection string 'OrdersMongo' não configurada.");

builder.Services.AddSingleton<IMongoClient>(
    _ => new MongoClient(mongoConnection));

builder.Services.AddSingleton(serviceProvider =>
    serviceProvider
        .GetRequiredService<IMongoClient>()
        .GetDatabase("ApiDePedidosRead"));

builder.Services.AddScoped<IOrderReadModelWriter, MongoOrderReadModelWriter>();
builder.Services.AddScoped<IOrderReadRepository, MongoOrderReadRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiDePedidos",
        Version = "v1"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Frontend");
app.MapControllers();

app.Run();