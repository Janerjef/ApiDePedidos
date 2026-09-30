using ApiDePedidos.Application.Abstractions;
using MediatR;

namespace ApiDePedidos.Application.Orders.Commands;

public sealed record DeleteOrderCommand(Guid OrderId) : IRequest<bool>;

public sealed class DeleteOrderHandler
    : IRequestHandler<DeleteOrderCommand, bool> {
    
    private readonly IOrderWriteRepository orders;
    private readonly IOrderReadModelWriter readModel;

    public DeleteOrderHandler(
        IOrderWriteRepository orders,
        IOrderReadModelWriter readModel) {
        
        this.orders = orders;
        this.readModel = readModel;
    }

    public async Task<bool> Handle(
        DeleteOrderCommand command,
        CancellationToken cancellationToken) {
        
        var order = await orders.GetByIdAsync(
            command.OrderId,
            cancellationToken);

        if (order is null)
            return false;

        await orders.DeleteAsync(order, cancellationToken);
        await readModel.DeleteAsync(order.Id, cancellationToken);

        return true;
    }
}