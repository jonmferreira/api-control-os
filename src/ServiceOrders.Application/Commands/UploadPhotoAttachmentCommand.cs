using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Commands;

public sealed record UploadPhotoAttachmentCommand(
    string Url,
    PhotoAttachmentType Type,
    Guid? OrderOfServiceId,
    Guid? ChecklistResponseItemId);
