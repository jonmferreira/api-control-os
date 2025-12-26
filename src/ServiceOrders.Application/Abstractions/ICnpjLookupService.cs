using ServiceOrders.Application.Dtos.Cnpj;

namespace ServiceOrders.Application.Abstractions;

public interface ICnpjLookupService
{
    Task<CnpjCompanyDto?> GetCompanyAsync(string cnpj, CancellationToken cancellationToken = default);
}
