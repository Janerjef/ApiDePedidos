using ApiDePedidos.Application.Orders.Commands;
using ApiDePedidos.Application.Orders.Dtos;
using ApiDePedidos.Application.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ApiDePedidos.Api.Controllers;
// recebe os pedidos http e encaminha para o midiaR
[ApiController] // construtor
[Route("api/orders")]
public sealed class OrdersController : ControllerBase {
    private readonly ISender sender;

    public OrdersController(ISender sender) // construtor
    {
        this.sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(  //POST - recebe os dados do pedido
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderId = await sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderId },
            new { id = orderId });
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderSummaryDto>>> GetAll(  //GET - lista todos os pedidos ( listagem resumida )
        CancellationToken cancellationToken)
    {
        var orders = await sender.Send(
            new GetOrdersQuery(),
            cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")] 
    public async Task<ActionResult<OrderDetailsDto>> GetById(   // GET - busca um pedido específico pelo ID 
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await sender.Send(
            new GetOrderByIdQuery(id),
            cancellationToken);

        if (order is null)
            return NotFound();

        return Ok(order);
    }
    
    public sealed record UpdateOrderRequest( // DTO - validação da lista de itens recebida no PUt
        [Required, MinLength(1)]
        IReadOnlyCollection<UpdateOrderItemRequest> Items);

    public sealed record UpdateOrderItemRequest( // DTO - validação individual de item no PUT
        Guid ItemId,
        [Range(1, int.MaxValue)]
        int Quantity);
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update( // PUT - atualiza a quantidade dos itens do pedido pelo ID
        Guid id,
        [FromBody] UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrderCommand(
            id,
            request.Items
                .Select(item => new UpdateOrderLine(item.ItemId, item.Quantity))
                .ToArray());

        var result = await sender.Send(command, cancellationToken);

        return result switch
        {
            UpdateOrderResult.Updated => NoContent(),
            UpdateOrderResult.NotFound => NotFound(),
            _ => BadRequest("Os itens enviados não correspondem aos itens do pedido.")
        };
    }

    [HttpDelete("{id:guid}")] 
    public async Task<IActionResult> Delete( // DELETE - apaga pedido pelo id
        Guid id,
        CancellationToken cancellationToken)
    {
        var deleted = await sender.Send(
            new DeleteOrderCommand(id),
            cancellationToken);

        return deleted ? NoContent() : NotFound();
    }
    
}