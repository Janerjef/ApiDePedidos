using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Application.Orders.Dtos;
using MediatR;

namespace ApiDePedidos.Application.Orders.Queries;
//listagem completa mas resumida
public sealed class GetOrdersHandler
    : IRequestHandler<GetOrdersQuery, IReadOnlyCollection<OrderSummaryDto>> {
    private readonly IOrderReadRepository repository;

    public GetOrdersHandler(IOrderReadRepository repository) {
        this.repository = repository;
    }

    public Task<IReadOnlyCollection<OrderSummaryDto>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken){
        return repository.GetAllAsync(cancellationToken);
    }
}