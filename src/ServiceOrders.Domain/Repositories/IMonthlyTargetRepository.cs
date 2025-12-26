using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IMonthlyTargetRepository
{
    Task<MonthlyTarget?> GetAsync(int year, int month, CancellationToken cancellationToken = default);

    Task<MonthlyTarget> UpsertAsync(int year, int month, int targetEntries, CancellationToken cancellationToken = default);
}
