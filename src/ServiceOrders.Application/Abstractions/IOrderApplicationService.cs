using ServiceOrders.Application.Commands;
using ServiceOrders.Application.Dtos;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Abstractions;

public interface IOrderApplicationService
{
    Task<OrderOfServiceDto> CreateAsync(CreateOrderOfServiceCommand command, CancellationToken cancellationToken = default);
    Task<OrderOfServiceDto> UpdateAsync(UpdateOrderOfServiceCommand command, CancellationToken cancellationToken = default);
    Task<OrderOfServiceDto> ChangeStatusAsync(ChangeOrderStatusCommand command, CancellationToken cancellationToken = default);
    Task<OrderOfServiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderOfServiceDto>> GetAsync(OrderStatus? status, string? technician, CancellationToken cancellationToken = default);
    Task<ChecklistTemplateDto> PublishTemplateAsync(PublishChecklistTemplateCommand command, CancellationToken cancellationToken = default);
    Task<ChecklistTemplateDto?> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ChecklistTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken = default);
    Task<ChecklistResponseDto> RegisterChecklistResponseAsync(RegisterChecklistResponseCommand command, CancellationToken cancellationToken = default);
    Task<CustomInputComponentDto> CreateComponentAsync(CreateCustomInputComponentCommand command, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomInputComponentDto>> GetComponentsAsync(CancellationToken cancellationToken = default);
    Task<PhotoAttachmentDto> UploadAttachmentAsync(UploadPhotoAttachmentCommand command, CancellationToken cancellationToken = default);
}
