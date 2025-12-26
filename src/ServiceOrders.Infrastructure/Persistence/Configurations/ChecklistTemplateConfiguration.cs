using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplate> builder)
    {
        builder.HasKey(template => template.Id);

        builder.Property(template => template.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(template => template.PublishedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(template => template.Items)
            .WithOne(item => item.ChecklistTemplate)
            .HasForeignKey(item => item.ChecklistTemplateId);

        builder.HasData(SeedData.InitialChecklistTemplate);
    }
}
