namespace ServiceOrders.Application.Dtos;

public sealed record ChecklistResponseDto(
    Guid Id,
    Guid ChecklistTemplateId,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<ChecklistResponseItemDto> Items);
