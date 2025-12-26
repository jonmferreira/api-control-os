using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Requests;

public sealed class PublishChecklistTemplateRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string PublishedBy { get; init; } = string.Empty;

    public bool PublishImmediately { get; init; } = true;

    [MinLength(1)]
    public IReadOnlyCollection<PublishChecklistTemplateItemRequest> Items { get; init; } =
        Array.Empty<PublishChecklistTemplateItemRequest>();
}

public sealed class PublishChecklistTemplateItemRequest
{
    [Required]
    [MaxLength(500)]
    public string Description { get; init; } = string.Empty;

    public bool HasCustomInput { get; init; }

    public Guid? CustomInputComponentId { get; init; }

    [MaxLength(50)]
    public string? DefaultOutcome { get; init; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; init; }
}
