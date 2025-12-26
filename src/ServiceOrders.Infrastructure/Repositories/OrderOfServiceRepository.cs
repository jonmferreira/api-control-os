using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public sealed class OrderOfServiceRepository : IOrderOfServiceRepository
{
    private readonly ServiceOrdersDbContext _dbContext;

    public OrderOfServiceRepository(ServiceOrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(OrderOfService order, CancellationToken cancellationToken = default)
    {
        await _dbContext.OrdersOfService.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrderOfService order, CancellationToken cancellationToken = default)
    {
        _dbContext.OrdersOfService.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderOfService?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrdersOfService
            .Include(order => order.ChecklistResponse!)
                .ThenInclude(response => response.Items)
                .ThenInclude(item => item.Attachments)
            .Include(order => order.Attachments)
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderOfService>> GetByStatusAsync(
        OrderStatus? status,
        string? assignedTechnician,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OrdersOfService
            .Include(order => order.ChecklistResponse!)
                .ThenInclude(response => response.Items)
                .ThenInclude(item => item.Attachments)
            .Include(order => order.Attachments)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(order => order.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(assignedTechnician))
        {
            query = query.Where(order => order.AssignedTechnician == assignedTechnician);
        }

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }
}
