using ApiDePedidos.Domain.Entities;

namespace ApiDePedidos.Application.Abstractions;

public interface IOrderWriteRepository { 
    Task AddAsync(Order order, CancellationToken cancellationToken); // novo pedido sql
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);  
    Task SaveChangesAsync(CancellationToken cancellationToken); 
    Task DeleteAsync(Order order, CancellationToken cancellationToken);
}