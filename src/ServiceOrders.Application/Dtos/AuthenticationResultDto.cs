namespace ServiceOrders.Application.Dtos;

public sealed record AuthenticationResultDto(
    Guid UserId,
    string Name,
    string Email,
    string Role,
    string AccessToken,
    DateTimeOffset ExpiresAt,
    int ExpiresIn); // Segundos até expiração
