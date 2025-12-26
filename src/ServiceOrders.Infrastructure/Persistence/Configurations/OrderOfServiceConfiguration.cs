using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class OrderOfServiceConfiguration : IEntityTypeConfiguration<OrderOfService>
{
    public void Configure(EntityTypeBuilder<OrderOfService> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(order => order.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(order => order.AssignedTechnician)
            .HasMaxLength(200);

        builder.Property(order => order.ClosingNotes)
            .HasMaxLength(2000);

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(order => order.ChecklistResponse)
            .WithOne(response => response.OrderOfService)
            .HasForeignKey<ChecklistResponse>(response => response.OrderOfServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Attachments)
            .WithOne(attachment => attachment.OrderOfService)
            .HasForeignKey(attachment => attachment.OrderOfServiceId);
    }
}
