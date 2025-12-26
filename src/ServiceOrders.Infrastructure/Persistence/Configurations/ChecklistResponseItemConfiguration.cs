using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class ChecklistResponseItemConfiguration : IEntityTypeConfiguration<ChecklistResponseItem>
{
    public void Configure(EntityTypeBuilder<ChecklistResponseItem> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Outcome)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(item => item.CustomInputPayload)
            .HasColumnType("nvarchar(max)");

        builder.Property(item => item.Observation)
            .HasMaxLength(1000);

        builder.HasMany(item => item.Attachments)
            .WithOne(attachment => attachment.ChecklistResponseItem)
            .HasForeignKey(attachment => attachment.ChecklistResponseItemId);
    }
}
