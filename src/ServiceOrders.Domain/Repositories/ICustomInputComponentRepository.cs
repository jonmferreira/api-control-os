using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface ICustomInputComponentRepository
{
    Task AddAsync(CustomInputComponent component, CancellationToken cancellationToken = default);
    Task<CustomInputComponent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomInputComponent>> GetAllAsync(CancellationToken cancellationToken = default);
}
