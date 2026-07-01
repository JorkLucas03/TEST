using Api.Domain.Common;
using Api.Domain.ValueObjects;

namespace Api.Domain.Entities;

public class Product : Entity<ProductId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product(ProductId id, string name, string description, Money price) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    // Constructor requerido por EF Core
    #pragma warning disable CS8618
    private Product() {}
    #pragma warning restore CS8618

    public static Product Create(string name, string description, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del producto no puede estar vacío.", nameof(name));

        return new Product(ProductId.From(Guid.NewGuid()), name, description, price);
    }

    public void UpdateDetails(string name, string description, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(name));

        Name = name;
        Description = description;
        Price = price;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
