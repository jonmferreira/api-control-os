using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Commands;

public sealed record ChangeOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status,
    string? Notes,
    DateTimeOffset? Timestamp,
    string? ChangedBy);
