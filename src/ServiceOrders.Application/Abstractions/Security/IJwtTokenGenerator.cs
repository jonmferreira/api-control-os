using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    JwtTokenResult GenerateToken(User user);
}

public sealed record JwtTokenResult(string Token, DateTimeOffset ExpiresAt);
