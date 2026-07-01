using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Convertidor para UserId (Vogen)
        builder.Property(u => u.Id)
            .HasConversion<UserId.EfCoreValueConverter>()
            .ValueGeneratedNever();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        // Convertidor para Email (Vogen)
        builder.Property(u => u.Email)
            .HasConversion<Email.EfCoreValueConverter>()
            .HasMaxLength(255)
            .IsRequired();

        // Filtro de consulta global para ignorar registros borrados lógicamente (Soft Delete)
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Crear un índice único filtrado en la columna Email para usuarios activos
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("IsDeleted = 0");
    }
}
