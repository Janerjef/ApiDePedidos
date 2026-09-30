namespace ApiDePedidos.Application.Orders.Dtos;

public sealed record OrderItemDetailsDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);

public sealed record OrderDetailsDto(
    Guid Id,
    Guid CustomerId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemDetailsDto> Items);

public sealed record OrderSummaryDto(
    Guid Id,
    Guid CustomerId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount);