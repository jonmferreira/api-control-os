using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class ChecklistTemplateItemConfiguration : IEntityTypeConfiguration<ChecklistTemplateItem>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplateItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(item => item.DisplayOrder)
            .IsRequired();

        builder.Property(item => item.DefaultOutcome)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(item => item.CustomInputComponent)
            .WithMany()
            .HasForeignKey(item => item.CustomInputComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.InitialChecklistItems);
    }
}
