using ApiDePedidos.Application.Abstractions;
using ApiDePedidos.Application.Orders.Dtos;
using MediatR;

namespace ApiDePedidos.Application.Orders.Queries;
//busca um pedido pelo id e retorna os detalhes
public sealed class GetOrderByIdHandler
    : IRequestHandler<GetOrderByIdQuery, OrderDetailsDto?>  
{
    private readonly IOrderReadRepository repository;

    public GetOrderByIdHandler(IOrderReadRepository repository) { 
        this.repository = repository;
    }

    public Task<OrderDetailsDto?> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken) {
        
        return repository.GetByIdAsync(request.OrderId, cancellationToken);
    }
}