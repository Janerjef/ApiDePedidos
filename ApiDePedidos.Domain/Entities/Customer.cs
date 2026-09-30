using System.Net.Mail;

namespace ApiDePedidos.Domain.Entities;

public class Customer
{
    //Customer
    // ○ Id (PK)
    // ○
    // ○Name
    // Email
    // ○Phone

    public Guid Id { 
        get; 
        private set; 
    }

    public string Name {
        get;
        private set;
    }

    public string Email {
        get;
        private set;
    }

    public string Phone {
        get;
        private set;
    }
    
    protected Customer(){
        
    } 
    public Customer(Guid id, string name, string email, string phone)
    {
        if (id == Guid.Empty) {
            throw new ArgumentException("O id do Customer não pode estar vazio", nameof(id));
        }
        if (string.IsNullOrWhiteSpace(name)) {   
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(name));

        }

        if (string.IsNullOrWhiteSpace(phone)) {
            throw new ArgumentException("O telefone do cliente é obrigatório.", nameof(phone)); 

        }

        if (!MailAddress.TryCreate(email, out _)) {
            throw new ArgumentException("O email do cliente está invalido.", nameof(email)); // verifica @.com
        }
        
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
    }

}