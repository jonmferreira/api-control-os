using ServiceOrders.Application.Dtos;

namespace ServiceOrders.Application.Abstractions;

public interface ICepLookupService
{
    Task<CepAddressDto?> GetAddressByCepAsync(string cep, CancellationToken cancellationToken = default);
}
