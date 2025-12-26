using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Commands;

public sealed record PublishChecklistTemplateCommand(
    string Title,
    string PublishedBy,
    bool PublishImmediately,
    IReadOnlyCollection<PublishChecklistTemplateItemCommand> Items);

public sealed record PublishChecklistTemplateItemCommand(
    string Description,
    bool HasCustomInput,
    Guid? CustomInputComponentId,
    ChecklistOutcome? DefaultOutcome,
    int DisplayOrder);
