namespace ApiDePedidos.Domain.Entities;

public class Product
{
    //Produto (Product)
    // ○ Id (PK)
    // ○Name
    // ○Price

    public Guid Id
    {
        get;
        private set;
    }

    public string Name
    {
        get;
        private set;
    }

    public decimal Price
    {
        get;
        private set;
    }

    protected Product() {
        
    }

    public Product(Guid id, string name, decimal price) {
        if (id == Guid.Empty) {
            throw new ArgumentException(nameof(price), "O valor precisa ser maior que zero.");
        }

        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(name));
        }

        if (price <= 0) {
            throw new ArgumentOutOfRangeException("O valor precisa ser maior que zero.",nameof(price));
        }
        
        
        Id = id;
        Name = name;
        Price = price;
    }
    
}