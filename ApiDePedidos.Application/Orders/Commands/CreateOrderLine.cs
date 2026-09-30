using MediatR;

namespace ApiDePedidos.Application.Orders.Commands; //separa em pasta
public sealed record CreateOrderLine(Guid ProductId, int Quantity);

public sealed record CreateOrderCommand(
    IReadOnlyCollection<CreateOrderLine> Items
) : IRequest<Guid>; // reune os itens e declara ao midiaR