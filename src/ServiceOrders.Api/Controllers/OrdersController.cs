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
[Authorize(Policy = "TechnicianOrAdmin")]
[Route("api/[controller]")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderApplicationService _orders;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderApplicationService orders, ILogger<OrdersController> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderOfServiceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderOfServiceResponse>> CreateAsync(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var command = new CreateOrderOfServiceCommand(
            request.Title,
            request.Description,
            request.AssignedTechnician,
            request.OpenedAt);

        var dto = await _orders.CreateAsync(command, cancellationToken);
        _logger.LogInformation(
            "User {User} created order {OrderId} for technician {Technician}",
            User.Identity?.Name ?? "unknown",
            dto.Id,
            dto.AssignedTechnician ?? "unassigned");

        return CreatedAtAction(nameof(GetByIdAsync), new { id = dto.Id }, ToResponse(dto));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderOfServiceResponse>> UpdateAsync(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var command = new UpdateOrderOfServiceCommand(id, request.Title, request.Description, request.AssignedTechnician);
            var dto = await _orders.UpdateAsync(command, cancellationToken);
            _logger.LogInformation(
                "User {User} updated order {OrderId} details",
                User.Identity?.Name ?? "unknown",
                id);
            return Ok(ToResponse(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(OrderOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderOfServiceResponse>> ChangeStatusAsync(Guid id, [FromBody] ChangeOrderStatusRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
        {
            return BadRequest(new { message = "Invalid status value." });
        }

        try
        {
            var command = new ChangeOrderStatusCommand(id, status, request.Notes, request.Timestamp, User.Identity?.Name);
            var dto = await _orders.ChangeStatusAsync(command, cancellationToken);
            _logger.LogInformation(
                "User {User} changed order {OrderId} status to {Status} at {Timestamp}",
                User.Identity?.Name ?? "unknown",
                id,
                status,
                request.Timestamp ?? DateTimeOffset.UtcNow);
            return Ok(ToResponse(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderOfServiceResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<OrderOfServiceResponse>>> GetAllAsync(
        [FromQuery] string? status,
        [FromQuery] string? technician,
        CancellationToken cancellationToken)
    {
        OrderStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsed))
        {
            parsedStatus = parsed;
        }

        var orders = await _orders.GetAsync(parsedStatus, technician, cancellationToken);
        return Ok(orders.Select(ToResponse).ToArray());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderOfServiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderOfServiceResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound() : Ok(ToResponse(order));
    }

    private static OrderOfServiceResponse ToResponse(OrderOfServiceDto dto)
    {
        return new OrderOfServiceResponse
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            AssignedTechnician = dto.AssignedTechnician,
            OpenedAt = dto.OpenedAt,
            CompletedAt = dto.CompletedAt,
            RejectedAt = dto.RejectedAt,
            ClosingNotes = dto.ClosingNotes,
            Status = dto.Status,
            ChecklistResponse = dto.ChecklistResponse is null ? null : ToResponse(dto.ChecklistResponse),
            Attachments = dto.Attachments.Select(ToResponse).ToArray()
        };
    }

    private static ChecklistResponseResponse ToResponse(ChecklistResponseDto dto)
    {
        return new ChecklistResponseResponse
        {
            Id = dto.Id,
            ChecklistTemplateId = dto.ChecklistTemplateId,
            CreatedAt = dto.CreatedAt,
            Items = dto.Items.Select(ToResponse).ToArray()
        };
    }

    private static ChecklistResponseItemResponse ToResponse(ChecklistResponseItemDto dto)
    {
        return new ChecklistResponseItemResponse
        {
            Id = dto.Id,
            ChecklistTemplateItemId = dto.ChecklistTemplateItemId,
            Outcome = dto.Outcome,
            CustomInputPayload = dto.CustomInputPayload,
            Observation = dto.Observation,
            Attachments = dto.Attachments.Select(ToResponse).ToArray()
        };
    }

    private static PhotoAttachmentResponse ToResponse(PhotoAttachmentDto dto)
    {
        return new PhotoAttachmentResponse
        {
            Id = dto.Id,
            Url = dto.Url,
            Type = dto.Type,
            OrderOfServiceId = dto.OrderOfServiceId,
            ChecklistResponseItemId = dto.ChecklistResponseItemId,
            CreatedAt = dto.CreatedAt
        };
    }
}
