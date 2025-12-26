using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IChecklistResponseRepository
{
    Task AddAsync(ChecklistResponse response, CancellationToken cancellationToken = default);
    Task<ChecklistResponse?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<ChecklistResponseItem?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
}
