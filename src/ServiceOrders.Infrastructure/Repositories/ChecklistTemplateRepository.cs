using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public sealed class ChecklistTemplateRepository : IChecklistTemplateRepository
{
    private readonly ServiceOrdersDbContext _dbContext;

    public ChecklistTemplateRepository(ServiceOrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(ChecklistTemplate template, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChecklistTemplates.AddAsync(template, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChecklistTemplate?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChecklistTemplates
            .Include(template => template.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(template => template.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChecklistTemplate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChecklistTemplates
            .Include(template => template.Items)
            .AsNoTracking()
            .OrderByDescending(template => template.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task PublishAsync(ChecklistTemplate template, CancellationToken cancellationToken = default)
    {
        _dbContext.ChecklistTemplates.Update(template);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
