using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        // Configurar el ValueConverter generado por Vogen para ProductId
        builder
            .Property(p => p.Id)
            .HasConversion<ProductId.EfCoreValueConverter>()
            .ValueGeneratedNever();

        builder.Property(p => p.Name).HasMaxLength(150).IsRequired();

        builder.Property(p => p.Description).HasMaxLength(500);

        // Mapear el ValueObject Money de manera embebida (Owned Types)
        builder.OwnsOne(
            p => p.Price,
            price =>
            {
                price
                    .Property(m => m.Amount)
                    .HasColumnName("PriceAmount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                price
                    .Property(m => m.Currency)
                    .HasColumnName("PriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );
    }
}
