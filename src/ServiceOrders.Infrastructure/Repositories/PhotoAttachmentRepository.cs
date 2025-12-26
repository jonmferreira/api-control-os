using Microsoft.EntityFrameworkCore;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Infrastructure.Persistence;

namespace ServiceOrders.Infrastructure.Repositories;

public sealed class PhotoAttachmentRepository : IPhotoAttachmentRepository
{
    private readonly ServiceOrdersDbContext _dbContext;

    public PhotoAttachmentRepository(ServiceOrdersDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task AddAsync(PhotoAttachment attachment, CancellationToken cancellationToken = default)
    {
        await _dbContext.PhotoAttachments.AddAsync(attachment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PhotoAttachment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhotoAttachments
            .AsNoTracking()
            .Where(attachment => attachment.OrderOfServiceId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PhotoAttachment>> GetByChecklistItemIdAsync(Guid checklistResponseItemId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PhotoAttachments
            .AsNoTracking()
            .Where(attachment => attachment.ChecklistResponseItemId == checklistResponseItemId)
            .ToListAsync(cancellationToken);
    }
}
