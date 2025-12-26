using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class CustomInputComponentConfiguration : IEntityTypeConfiguration<CustomInputComponent>
{
    public void Configure(EntityTypeBuilder<CustomInputComponent> builder)
    {
        builder.HasKey(component => component.Id);

        builder.Property(component => component.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(component => component.JsonBody)
            .IsRequired();

        builder.HasData(SeedData.DadosCarroComponent);
    }
}
