using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Infrastructure.Persistence;

public sealed class ServiceOrdersDbContext : DbContext
{
    public ServiceOrdersDbContext(DbContextOptions<ServiceOrdersDbContext> options)
        : base(options)
    {
    }

    public DbSet<OrderOfService> OrdersOfService => Set<OrderOfService>();
    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItems => Set<ChecklistTemplateItem>();
    public DbSet<ChecklistResponse> ChecklistResponses => Set<ChecklistResponse>();
    public DbSet<ChecklistResponseItem> ChecklistResponseItems => Set<ChecklistResponseItem>();
    public DbSet<CustomInputComponent> CustomInputComponents => Set<CustomInputComponent>();
    public DbSet<PhotoAttachment> PhotoAttachments => Set<PhotoAttachment>();
    public DbSet<MonthlyTarget> MonthlyTargets => Set<MonthlyTarget>();
    public DbSet<User> Users => Set<User>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiceOrdersDbContext).Assembly);
    }
}
