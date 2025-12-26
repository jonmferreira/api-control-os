using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IChecklistTemplateRepository
{
    Task AddAsync(ChecklistTemplate template, CancellationToken cancellationToken = default);
    Task<ChecklistTemplate?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChecklistTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task PublishAsync(ChecklistTemplate template, CancellationToken cancellationToken = default);
}
