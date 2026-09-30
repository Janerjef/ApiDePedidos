using ApiDePedidos.Application.Orders.Dtos;

namespace ApiDePedidos.Application.Abstractions;

public interface IOrderReadRepository{ // consulta rapida
    Task<OrderDetailsDto?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken); // detalhes do pedido pelo id
    Task<IReadOnlyCollection<OrderSummaryDto>> GetAllAsync(CancellationToken cancellationToken); // busca lista resumida
}