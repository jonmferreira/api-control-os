using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Api.Models.Responses;
using ServiceOrders.Application.Abstractions;
using ServiceOrders.Application.Commands;
using ServiceOrders.Application.Dtos;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Authorize(Policy = "AdminOnly")]
[Route("api/checklist-templates")]
public sealed class ChecklistTemplatesController : ControllerBase
{
    private readonly IOrderApplicationService _orders;
    private readonly ILogger<ChecklistTemplatesController> _logger;

    public ChecklistTemplatesController(IOrderApplicationService orders, ILogger<ChecklistTemplatesController> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChecklistTemplateResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ChecklistTemplateResponse>> PublishAsync([FromBody] PublishChecklistTemplateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new PublishChecklistTemplateCommand(
            request.Title,
            request.PublishedBy,
            request.PublishImmediately,
            request.Items.Select(item => new PublishChecklistTemplateItemCommand(
                item.Description,
                item.HasCustomInput,
                item.CustomInputComponentId,
                ParseOutcome(item.DefaultOutcome),
                item.DisplayOrder)).ToArray());

        var dto = await _orders.PublishTemplateAsync(command, cancellationToken);
        _logger.LogInformation(
            "User {User} published checklist template {TemplateId} ({Title})",
            User.Identity?.Name ?? "unknown",
            dto.Id,
            dto.Title);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = dto.Id }, ToResponse(dto));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ChecklistTemplateResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ChecklistTemplateResponse>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var templates = await _orders.GetTemplatesAsync(cancellationToken);
        return Ok(templates.Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ChecklistTemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChecklistTemplateResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var template = await _orders.GetTemplateByIdAsync(id, cancellationToken);
        return template is null ? NotFound() : Ok(ToResponse(template));
    }

    private static ChecklistOutcome? ParseOutcome(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return Enum.TryParse<ChecklistOutcome>(value, true, out var outcome) ? outcome : null;
    }

    private static ChecklistTemplateResponse ToResponse(ChecklistTemplateDto dto)
    {
        return new ChecklistTemplateResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            PublishedBy = dto.PublishedBy,
            IsPublished = dto.IsPublished,
            UpdatedAt = dto.UpdatedAt,
            Items = dto.Items.Select(item => new ChecklistTemplateItemResponse
            {
                Id = item.Id,
                Description = item.Description,
                HasCustomInput = item.HasCustomInput,
                CustomInputComponentId = item.CustomInputComponentId,
                DefaultOutcome = item.DefaultOutcome,
                DisplayOrder = item.DisplayOrder
            }).ToArray()
        };
    }
}
