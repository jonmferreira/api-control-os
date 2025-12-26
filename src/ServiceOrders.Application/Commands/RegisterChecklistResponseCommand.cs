using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Commands;

public sealed record RegisterChecklistResponseCommand(
    Guid OrderOfServiceId,
    Guid ChecklistTemplateId,
    IReadOnlyCollection<RegisterChecklistResponseItemCommand> Items);

public sealed record RegisterChecklistResponseItemCommand(
    Guid ChecklistTemplateItemId,
    ChecklistOutcome Outcome,
    string? CustomInputPayload,
    string? Observation,
    IReadOnlyCollection<string> PhotoUrls);
