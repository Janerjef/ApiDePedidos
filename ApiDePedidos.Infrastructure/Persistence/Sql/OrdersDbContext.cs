using ApiDePedidos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiDePedidos.Infrastructure.Persistence.Sql;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasKey(order => order.Id);

        modelBuilder.Entity<Order>()
            .HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId);

        modelBuilder.Entity<Order>()
            .Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        modelBuilder.Entity<OrderItem>()
            .HasKey(item => item.Id);

        modelBuilder.Entity<OrderItem>()
            .Property(item => item.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(item => item.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(order => order.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<Order>()
            .HasOne<Customer>()
            .WithMany()
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Customer>().HasData(
            new Customer(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Cliente de Teste",
                "cliente@exemplo.com",
                "71999990000"));

        modelBuilder.Entity<Product>().HasData(
            new Product(
                Guid.Parse("00000000-0000-0000-0000-000000000001"),
                "Teclado",
                150.00m),
            new Product(
                Guid.Parse("00000000-0000-0000-0000-000000000002"),
                "Mouse",
                80.00m),
            new Product(
                Guid.Parse("00000000-0000-0000-0000-000000000003"),
                "Monitor",
                900.00m));
    }
}