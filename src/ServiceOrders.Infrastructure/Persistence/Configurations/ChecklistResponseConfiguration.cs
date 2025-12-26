using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class ChecklistResponseConfiguration : IEntityTypeConfiguration<ChecklistResponse>
{
    public void Configure(EntityTypeBuilder<ChecklistResponse> builder)
    {
        builder.HasKey(response => response.Id);

        builder.HasMany(response => response.Items)
            .WithOne(item => item.ChecklistResponse)
            .HasForeignKey(item => item.ChecklistResponseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
