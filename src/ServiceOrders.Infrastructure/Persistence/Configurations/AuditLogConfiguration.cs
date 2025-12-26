using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Action)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(log => log.ResourceType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(log => log.PerformedBy)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(log => log.Details)
            .IsRequired()
            .HasMaxLength(2000);
    }
}
