namespace ServiceOrders.Application.Dtos;

public sealed record ChecklistResponseItemDto(
    Guid Id,
    Guid ChecklistTemplateItemId,
    string Outcome,
    string? CustomInputPayload,
    string? Observation,
    IReadOnlyCollection<PhotoAttachmentDto> Attachments);
