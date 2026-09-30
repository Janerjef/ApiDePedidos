using ApiDePedidos.Application.Orders.Dtos;
using MediatR;

namespace ApiDePedidos.Application.Orders.Queries;

public sealed record GetOrdersQuery
    : IRequest<IReadOnlyCollection<OrderSummaryDto>>;

