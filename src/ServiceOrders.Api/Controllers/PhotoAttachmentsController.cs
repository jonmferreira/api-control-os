using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Api.Models.Responses;
using ServiceOrders.Application.Abstractions;
using ServiceOrders.Application.Commands;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Api.Controllers;

[ApiController]
[Authorize(Policy = "TechnicianOrAdmin")]
[Route("api/photos")]
public sealed class PhotoAttachmentsController : ControllerBase
{
    private readonly IOrderApplicationService _orders;
    private readonly ILogger<PhotoAttachmentsController> _logger;

    public PhotoAttachmentsController(IOrderApplicationService orders, ILogger<PhotoAttachmentsController> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PhotoAttachmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PhotoAttachmentResponse>> UploadAsync([FromBody] UploadPhotoAttachmentRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!Enum.TryParse<PhotoAttachmentType>(request.Type, true, out var type))
        {
            return BadRequest(new { message = "Invalid photo attachment type." });
        }

        try
        {
            var command = new UploadPhotoAttachmentCommand(request.Url, type, request.OrderOfServiceId, request.ChecklistResponseItemId);
            var dto = await _orders.UploadAttachmentAsync(command, cancellationToken);

            _logger.LogInformation(
                "User {User} uploaded photo {PhotoId} ({Type})",
                User.Identity?.Name ?? "unknown",
                dto.Id,
                dto.Type);

            return CreatedAtAction(nameof(UploadAsync), new { id = dto.Id }, new PhotoAttachmentResponse
            {
                Id = dto.Id,
                Url = dto.Url,
                Type = dto.Type,
                OrderOfServiceId = dto.OrderOfServiceId,
                ChecklistResponseItemId = dto.ChecklistResponseItemId,
                CreatedAt = dto.CreatedAt
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
