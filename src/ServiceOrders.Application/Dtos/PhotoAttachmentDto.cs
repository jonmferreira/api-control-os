namespace ServiceOrders.Application.Dtos;

public sealed record PhotoAttachmentDto(
    Guid Id,
    string Url,
    string Type,
    Guid? OrderOfServiceId,
    Guid? ChecklistResponseItemId,
    DateTimeOffset CreatedAt);
