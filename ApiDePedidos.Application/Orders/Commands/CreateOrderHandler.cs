using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Domain.Entities;
using MediatR;

namespace ApiDePedidos.Application.Orders.Commands;
//comande l
public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>{ //recebe os itens reunidos e retorna o id do pedido
    private readonly IOrderWriteRepository orders;
    private readonly IOrderReferenceData referenceData;
    private readonly IOrderReadModelWriter readModel;

    public CreateOrderHandler(IOrderWriteRepository orders, IOrderReferenceData referenceData,
        IOrderReadModelWriter readModel) { // construtor
        this.orders = orders;
        this.referenceData = referenceData;
        this.readModel = readModel;
    }

    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken){

        if (command.Items is null || command.Items.Count == 0){
            throw new ArgumentException("O pedido precisa ter ao menos um item.", nameof(command));
        }

        var customer = await referenceData.GetFixedCustomerAsync(cancellationToken); // busca cliente no banco  

        var order = new Order(customer.Id); // nova entidade do pedido com id do cliente 

        foreach (var requestedItem in command.Items){ // percorre os itens solicitaos
            var product = await referenceData.GetProductAsync(requestedItem.ProductId, cancellationToken); // busca no banco

            if (product is null) {
                throw new InvalidOperationException("Produto não encontrado.");
            }


            var item = new OrderItem(
                product.Id,
                order.Id,
                product.Price,
                requestedItem.Quantity,
                product.Name);

            order.AdicionarItem(item);
        }

        await orders.AddAsync(order, cancellationToken); // periste o pedido no sqlserver 
        await readModel.UpsertAsync(order, cancellationToken); // atualiza a projeção de leitura no MongoDb

        return order.Id; 

    }
}
