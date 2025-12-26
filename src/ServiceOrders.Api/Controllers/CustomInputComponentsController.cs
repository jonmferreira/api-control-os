using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Api.Models.Responses;
using ServiceOrders.Application.Abstractions;
using ServiceOrders.Application.Commands;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/custom-components")]
public sealed class CustomInputComponentsController : ControllerBase
{
    private readonly IOrderApplicationService _orders;
    private readonly ILogger<CustomInputComponentsController> _logger;

    public CustomInputComponentsController(IOrderApplicationService orders, ILogger<CustomInputComponentsController> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomInputComponentResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CustomInputComponentResponse>> CreateAsync([FromBody] CreateCustomInputComponentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = await _orders.CreateComponentAsync(new CreateCustomInputComponentCommand(request.Name, request.JsonBody), cancellationToken);
        _logger.LogInformation(
            "User {User} created custom input component {ComponentId} ({Name})",
            User.Identity?.Name ?? "unknown",
            dto.Id,
            dto.Name);
        return CreatedAtAction(nameof(GetAllAsync), new { id = dto.Id }, new CustomInputComponentResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            JsonBody = dto.JsonBody,
            UpdatedAt = dto.UpdatedAt
        });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomInputComponentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CustomInputComponentResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var components = await _orders.GetComponentsAsync(cancellationToken);

        return Ok(components.Select(component => new CustomInputComponentResponse
        {
            Id = component.Id,
            Name = component.Name,
            JsonBody = component.JsonBody,
            UpdatedAt = component.UpdatedAt
        }).ToArray());
    }
}
