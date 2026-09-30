using ApiDePedidos.Application.Abstractions;
using MediatR;

namespace ApiDePedidos.Application.Orders.Commands;
public sealed record UpdateOrderLine(Guid ItemId, int Quantity);
public sealed record UpdateOrderCommand(
    Guid OrderId,
    IReadOnlyCollection<UpdateOrderLine> Items
) : IRequest<UpdateOrderResult>;

public enum UpdateOrderResult {
    Updated,
    NotFound,
    InvalidItems
}

public sealed class UpdateOrderHandler
    : IRequestHandler<UpdateOrderCommand, UpdateOrderResult> {
    
    private readonly IOrderWriteRepository orders;
    private readonly IOrderReadModelWriter readModel;

    public UpdateOrderHandler(
        IOrderWriteRepository orders,
        IOrderReadModelWriter readModel) {
        
        this.orders = orders;
        this.readModel = readModel;
    }

    public async Task<UpdateOrderResult> Handle(
        UpdateOrderCommand command,
        CancellationToken cancellationToken) {
        if (command.OrderId == Guid.Empty ||
            command.Items is null ||
            command.Items.Count == 0 ||
            command.Items.Any(item => item.ItemId == Guid.Empty || item.Quantity <= 0)) {
            return UpdateOrderResult.InvalidItems;
        }

        var order = await orders.GetByIdAsync(
            command.OrderId,
            cancellationToken);

        if (order is null) {
            return UpdateOrderResult.NotFound;
        }

        var requestedIds = command.Items.Select(item => item.ItemId).ToList();
        var existingIds = order.Items.Select(item => item.Id).ToList();

        if (requestedIds.Count != existingIds.Count ||
            requestedIds.Distinct().Count() != requestedIds.Count ||
            existingIds.Any(id => !requestedIds.Contains(id))) {
            
            return UpdateOrderResult.InvalidItems;
        }

        foreach (var item in command.Items) {
            order.Atualizar(item.ItemId, item.Quantity);
        }
        
        await orders.SaveChangesAsync(cancellationToken);
        await readModel.UpsertAsync(order, cancellationToken);

        return UpdateOrderResult.Updated;
    }
}