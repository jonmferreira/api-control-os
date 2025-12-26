using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ServiceOrders.Api.Mappings;
using ServiceOrders.Api.Models.Responses;
using ServiceOrders.Application.Abstractions;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Route("api/cnpj")]
[Authorize(Policy = "TechnicianOrAdmin")]
public sealed class CnpjController : ControllerBase
{
    private readonly ICnpjLookupService _cnpjLookupService;

    public CnpjController(ICnpjLookupService cnpjLookupService)
    {
        _cnpjLookupService = cnpjLookupService;
    }

    [HttpGet("{cnpj}")]
    [ProducesResponseType(typeof(CnpjCompanyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<CnpjCompanyResponse>> GetByCnpj(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _cnpjLookupService.GetCompanyAsync(cnpj, cancellationToken);
            if (company is null)
            {
                return NotFound();
            }

            return Ok(company.ToResponse());
        }
        catch (ArgumentException ex)
        {
            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid CNPJ provided.");
        }
        catch (HttpRequestException ex)
        {
            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway,
                title: "CNPJa API returned an error.");
        }
        catch (JsonException ex)
        {
            return Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status502BadGateway,
                title: "CNPJa API returned an unexpected payload.");
        }
    }
}
