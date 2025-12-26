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
[Route("api/checklist-responses")]
public sealed class ChecklistResponsesController : ControllerBase
{
    private readonly IOrderApplicationService _orders;
    private readonly ILogger<ChecklistResponsesController> _logger;

    public ChecklistResponsesController(IOrderApplicationService orders, ILogger<ChecklistResponsesController> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChecklistResponseResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ChecklistResponseResponse>> RegisterAsync([FromBody] RegisterChecklistResponseRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var command = new RegisterChecklistResponseCommand(
                request.OrderOfServiceId,
                request.ChecklistTemplateId,
                request.Items.Select(item =>
                {
                    if (!TryParseOutcome(item.Outcome, out var outcome))
                    {
                        throw new ArgumentException("Outcome must be Approved, Rejected or NotApplicable.");
                    }

                    return new RegisterChecklistResponseItemCommand(
                        item.ChecklistTemplateItemId,
                        outcome,
                        item.CustomInputPayload,
                        item.Observation,
                        item.PhotoUrls);
                }).ToArray());

            var dto = await _orders.RegisterChecklistResponseAsync(command, cancellationToken);
            _logger.LogInformation(
                "User {User} registered checklist response {ChecklistResponseId} for order {OrderId}",
                User.Identity?.Name ?? "unknown",
                dto.Id,
                request.OrderOfServiceId);
            return CreatedAtAction(nameof(RegisterAsync), new { id = dto.Id }, ToResponse(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private static bool TryParseOutcome(string value, out ChecklistOutcome outcome)
    {
        if (!Enum.TryParse(value, true, out outcome))
        {
            return false;
        }

        return true;
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
