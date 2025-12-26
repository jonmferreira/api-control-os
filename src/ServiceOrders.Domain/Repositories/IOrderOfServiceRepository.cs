using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IOrderOfServiceRepository
{
    Task AddAsync(OrderOfService order, CancellationToken cancellationToken = default);
    Task UpdateAsync(OrderOfService order, CancellationToken cancellationToken = default);
    Task<OrderOfService?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderOfService>> GetByStatusAsync(
        OrderStatus? status,
        string? assignedTechnician,
        CancellationToken cancellationToken = default);
}
