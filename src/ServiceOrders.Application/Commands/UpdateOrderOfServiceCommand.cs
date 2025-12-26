namespace ServiceOrders.Application.Commands;

public sealed record UpdateOrderOfServiceCommand(
    Guid OrderId,
    string Title,
    string Description,
    string? AssignedTechnician);
