using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IPhotoAttachmentRepository
{
    Task AddAsync(PhotoAttachment attachment, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PhotoAttachment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PhotoAttachment>> GetByChecklistItemIdAsync(Guid checklistResponseItemId, CancellationToken cancellationToken = default);
}
