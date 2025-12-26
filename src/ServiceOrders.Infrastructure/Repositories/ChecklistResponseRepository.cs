using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public sealed class ChecklistResponseRepository : IChecklistResponseRepository
{
    private readonly ServiceOrdersDbContext _dbContext;

    public ChecklistResponseRepository(ServiceOrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(ChecklistResponse response, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChecklistResponses.AddAsync(response, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChecklistResponse?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChecklistResponses
            .Include(response => response.Items)
                .ThenInclude(item => item.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(response => response.OrderOfServiceId == orderId, cancellationToken);
    }

    public async Task<ChecklistResponseItem?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChecklistResponseItems
            .Include(item => item.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == itemId, cancellationToken);
    }
}
