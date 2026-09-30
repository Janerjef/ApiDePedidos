using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiDePedidos.Infrastructure.Persistence.Sql;

public sealed class SqlOrderWriteRepository : IOrderWriteRepository
{
    private readonly OrdersDbContext db;

    public SqlOrderWriteRepository(OrdersDbContext db)
    {
        this.db = db;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await db.Orders.AddAsync(order, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return db.Orders
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Order order, CancellationToken cancellationToken)
    {
        db.Orders.Remove(order);
        await db.SaveChangesAsync(cancellationToken);
    }
}