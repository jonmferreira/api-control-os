using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Repositories;

public interface IPasswordResetTokenRepository
{
    Task AddAsync(PasswordResetToken token, CancellationToken cancellationToken = default);

    Task<PasswordResetToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task UpdateAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
}
