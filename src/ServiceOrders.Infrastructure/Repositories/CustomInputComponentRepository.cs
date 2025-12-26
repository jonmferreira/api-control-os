using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public sealed class CustomInputComponentRepository : ICustomInputComponentRepository
{
    private readonly ServiceOrdersDbContext _dbContext;

    public CustomInputComponentRepository(ServiceOrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(CustomInputComponent component, CancellationToken cancellationToken = default)
    {
        await _dbContext.CustomInputComponents.AddAsync(component, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CustomInputComponent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.CustomInputComponents.AsNoTracking().FirstOrDefaultAsync(component => component.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CustomInputComponent>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.CustomInputComponents.AsNoTracking().OrderByDescending(component => component.UpdatedAt).ToListAsync(cancellationToken);
    }
}
