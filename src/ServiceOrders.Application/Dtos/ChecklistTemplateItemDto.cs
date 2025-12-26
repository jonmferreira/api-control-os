namespace ServiceOrders.Application.Dtos;

public sealed record ChecklistTemplateItemDto(
    Guid Id,
    string Description,
    bool HasCustomInput,
    Guid? CustomInputComponentId,
    string? DefaultOutcome,
    int DisplayOrder);
