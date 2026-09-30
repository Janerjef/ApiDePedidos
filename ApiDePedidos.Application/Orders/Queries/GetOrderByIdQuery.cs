using ApiDePedidos.Application.Orders.Dtos;
using MediatR;

namespace ApiDePedidos.Application.Orders.Queries;

public sealed record GetOrderByIdQuery(Guid OrderId)
    : IRequest<OrderDetailsDto?>;