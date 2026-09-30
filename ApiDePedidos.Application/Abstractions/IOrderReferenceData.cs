using ApiDePedidos.Domain.Entities;

namespace ApiDePedidos.Application.Abstractions;

public interface IOrderReferenceData { 
    Task <Customer> GetFixedCustomerAsync(CancellationToken cancellationToken); // busca cliente 
    public Task<Product?> GetProductAsync(Guid productId, CancellationToken cancellationToken); // busca dados do produto pelo id

}