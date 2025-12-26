using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Requests;

public sealed class RegisterChecklistResponseRequest
{
    [Required]
    public Guid OrderOfServiceId { get; init; }

    [Required]
    public Guid ChecklistTemplateId { get; init; }

    [MinLength(1)]
    public IReadOnlyCollection<RegisterChecklistResponseItemRequest> Items { get; init; } =
        Array.Empty<RegisterChecklistResponseItemRequest>();
}

public sealed class RegisterChecklistResponseItemRequest
{
    [Required]
    public Guid ChecklistTemplateItemId { get; init; }

    [Required]
    public string Outcome { get; init; } = string.Empty;

    public string? CustomInputPayload { get; init; }

    [MaxLength(1000)]
    public string? Observation { get; init; }

    public IReadOnlyCollection<string> PhotoUrls { get; init; } = Array.Empty<string>();
}
