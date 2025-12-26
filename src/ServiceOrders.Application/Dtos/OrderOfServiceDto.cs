namespace ServiceOrders.Application.Dtos;

public sealed record OrderOfServiceDto(
    Guid Id,
    string Title,
    string Description,
    string? AssignedTechnician,
    DateTimeOffset OpenedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? RejectedAt,
    string? ClosingNotes,
    string Status,
    ChecklistResponseDto? ChecklistResponse,
    IReadOnlyCollection<PhotoAttachmentDto> Attachments);
