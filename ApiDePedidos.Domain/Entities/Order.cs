using System.ComponentModel;

namespace ApiDePedidos.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> items = new();

    
    public Guid Id{
        get; 
        private set;
    }

    public Guid CustomerId {
        get;
        private set;
    }

    public DateTime OrderDate {
        get;
        private set;
    }
    

    public OrderStatus OrderStatus {
        get; 
        private set;
    }

    public decimal TotalAmount {
        get;
        private set;
    }
    
    public IReadOnlyCollection<OrderItem> Items => items.AsReadOnly();

   

    protected Order() {
        
    }

    public Order(Guid customerId) {
        if (customerId == Guid.Empty) {
            throw new ArgumentException("O identificador do cliente não pode estar vazio",nameof(customerId));
        }
        
        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        OrderStatus = OrderStatus.AguardandoPagamento;
        TotalAmount = 0;
    }
    
    
    public void AdicionarItem(OrderItem item){
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Selecione o item para adicionar.");

        if (item.OrderId != Id)
            throw new ArgumentException("O item pertence a outro pedido.", nameof(item));

        var produtoExistente = items.FirstOrDefault(
            existing => existing.ProductId == item.ProductId && existing.UnitPrice == item.UnitPrice);

        if (produtoExistente != null) {
            produtoExistente.SomaQuantidade(item.Quantity);
        }
        else {
            items.Add(item);
        }

        RecalcularTotalAmount();
        
    }
    
    public void Remover(OrderItem item)
    {
        if (item == null) {
            throw new ArgumentNullException(nameof(item), "Selecione o item para remover.");
        } 
         bool removido = items.Remove(item);
         RecalcularTotalAmount();
         
         if (!removido) {
             throw new InvalidOperationException("O item não foi encontrado na lista");
         }
    }

    public void Atualizar(Guid itemId, int novaquantidade) {
        if (itemId == Guid.Empty) {
            throw new ArgumentException( "Selecione o item para atualizar.", nameof(itemId));
        }
        
        var item = items.FirstOrDefault(existing=> existing.Id == itemId);

        if (item == null) {
         throw new InvalidOperationException("O item não foi  encontrado na lista");   
        }

        item.AlterarQuantidade(novaquantidade);
        RecalcularTotalAmount();
    }
    private void RecalcularTotalAmount() {
        TotalAmount = items.Sum(item => item.TotalPrice);
    }
    
    
    
}