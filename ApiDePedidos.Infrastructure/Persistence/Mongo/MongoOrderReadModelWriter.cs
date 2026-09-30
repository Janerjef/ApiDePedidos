using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Domain.Entities;
using MongoDB.Driver;

namespace ApiDePedidos.Infrastructure.Persistence.Mongo;

public sealed class MongoOrderReadModelWriter : IOrderReadModelWriter
{
    private readonly IMongoCollection<MongoOrderDocument> orders;

    public MongoOrderReadModelWriter(IMongoDatabase database)
    {
        orders = database.GetCollection<MongoOrderDocument>("Orders");
    }

    public Task UpsertAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        var document = new MongoOrderDocument
        {
            Id = order.Id.ToString(),
            CustomerId = order.CustomerId.ToString(),
            OrderDate = order.OrderDate,
            Status = order.OrderStatus.ToString(),
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new MongoOrderItemDocument
            {
                Id = item.Id.ToString(),
                ProductId = item.ProductId.ToString(),
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice
            }).ToList()
        };

        return orders.ReplaceOneAsync(
            item => item.Id == document.Id,
            document,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);
    }
    public Task DeleteAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        return orders.DeleteOneAsync(
            item => item.Id == orderId.ToString(),
            cancellationToken);
    }
}