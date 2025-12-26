using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken cancellationToken = default);
}
