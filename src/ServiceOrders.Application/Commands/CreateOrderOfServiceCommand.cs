namespace ServiceOrders.Application.Commands;

public sealed record CreateOrderOfServiceCommand(
    string Title,
    string Description,
    string? AssignedTechnician,
    DateTimeOffset? OpenedAt);
