using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Application.Orders.Dtos;
using MongoDB.Driver;

namespace ApiDePedidos.Infrastructure.Persistence.Mongo;

public sealed class MongoOrderReadRepository : IOrderReadRepository
{
    private readonly IMongoCollection<MongoOrderDocument> orders;

    public MongoOrderReadRepository(IMongoDatabase database)
    {
        orders = database.GetCollection<MongoOrderDocument>("Orders");
    }

    public async Task<OrderDetailsDto?> GetByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var document = await orders
            .Find(order => order.Id == orderId.ToString())
            .FirstOrDefaultAsync(cancellationToken);

        if (document is null)
            return null;

        var items = document.Items
            .Select(item => new OrderItemDetailsDto(
                Guid.Parse(item.Id),
                Guid.Parse(item.ProductId),
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                item.TotalPrice))
            .ToList();

        return new OrderDetailsDto(
            Guid.Parse(document.Id),
            Guid.Parse(document.CustomerId),
            document.OrderDate,
            document.Status,
            document.TotalAmount,
            items);
    }

    public async Task<IReadOnlyCollection<OrderSummaryDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var documents = await orders
            .Find(_ => true)
            .ToListAsync(cancellationToken);

        return documents
            .Select(document => new OrderSummaryDto(
                Guid.Parse(document.Id),
                Guid.Parse(document.CustomerId),
                document.OrderDate,
                document.Status,
                document.TotalAmount))
            .ToList();
    }
}