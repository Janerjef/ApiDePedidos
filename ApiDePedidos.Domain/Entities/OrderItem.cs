using ApiDePedidos.Domain.Entities;
namespace ApiDePedidos.Domain.Entities;

public class OrderItem
{
    public Guid Id
    {
        get;
        private set;
    }

    public Guid OrderId
    {
        get;
        private set;
    }

    public Guid ProductId
    {
        get; 
        private set;
    }

    public string ProductName { get; 
        private set; 
    } = string.Empty;

    public decimal UnitPrice
    {
        get;
        private set;
    }

    public int Quantity
    {
        get;
        private set;
    }

    public decimal TotalPrice
    {
        get;
        private set;
    }
    
    protected OrderItem(){
        
    }

    public OrderItem(Guid productId, Guid orderId, decimal unitPrice, int quantity, string productName)
    {
        
        if (productId == Guid.Empty) {
            throw new ArgumentException("O id do produto não pode estar vazio", nameof(productId));
        }

        if (orderId == Guid.Empty) {
            throw new ArgumentException("O id do pedido não pode estar vazio", nameof(orderId));
        }

        if (quantity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser maior que zero.");
            
        }

        if (unitPrice <= 0) {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "O preço deve ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(productName)) {
            throw new ArgumentException("O nome do produto não pode estar vazio", nameof(productName));
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;

        RecalcularTotal();
    }

    public void SomaQuantidade(int quantidadeAdicional){
        if (quantidadeAdicional <= 0) {
            throw new ArgumentOutOfRangeException(nameof(quantidadeAdicional), "A quantidade deve ser maior que zero.");
        }

        Quantity += quantidadeAdicional;
        RecalcularTotal();
    }

    public void AlterarQuantidade(int novaquantidade) {
        if (novaquantidade <= 0) {
            throw new ArgumentOutOfRangeException(nameof(novaquantidade), "A quantidade deve ser maior que zero.");
        }
        Quantity = novaquantidade;
        RecalcularTotal();
    }

    public void RecalcularTotal() { 
        TotalPrice = Quantity* UnitPrice;
    }
}
