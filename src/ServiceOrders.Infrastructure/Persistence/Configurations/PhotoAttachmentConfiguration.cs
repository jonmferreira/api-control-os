using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence.Configurations;

public sealed class PhotoAttachmentConfiguration : IEntityTypeConfiguration<PhotoAttachment>
{
    public void Configure(EntityTypeBuilder<PhotoAttachment> builder)
    {
        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment => attachment.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(attachment => attachment.Type)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
