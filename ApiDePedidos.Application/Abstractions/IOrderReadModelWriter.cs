using ApiDePedidos.Domain.Entities;

namespace ApiDePedidos.Application.Abstractions;

public interface IOrderReadModelWriter // para manter MongoDb atualizado
{
    Task UpsertAsync(Order order, CancellationToken cancellationToken); // mantem autualizado
    Task DeleteAsync(Guid orderId, CancellationToken cancellationToken);
}