using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiDePedidos.Infrastructure.Persistence.Sql;

public sealed class SqlOrderReferenceData : IOrderReferenceData
{
    private readonly OrdersDbContext db;

    public SqlOrderReferenceData(OrdersDbContext db)
    {
        this.db = db;
    }

    public async Task<Customer> GetFixedCustomerAsync(
        CancellationToken cancellationToken)
    {
        var customer = await db.Customers
            .SingleOrDefaultAsync(cancellationToken);

        return customer
               ?? throw new InvalidOperationException(
                   "O cliente fixo não foi cadastrado no banco.");
    }

    public Task<Product?> GetProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return db.Products.SingleOrDefaultAsync(
            product => product.Id == productId,
            cancellationToken);
    }
}