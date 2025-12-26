namespace ServiceOrders.Application.Dtos;

public sealed record ChecklistTemplateDto(
    Guid Id,
    string Title,
    string PublishedBy,
    bool IsPublished,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<ChecklistTemplateItemDto> Items);
