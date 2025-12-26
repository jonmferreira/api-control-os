namespace ServiceOrders.Application.Dtos;

public sealed record CustomInputComponentDto(
    Guid Id,
    string Name,
    string JsonBody,
    DateTimeOffset UpdatedAt);
