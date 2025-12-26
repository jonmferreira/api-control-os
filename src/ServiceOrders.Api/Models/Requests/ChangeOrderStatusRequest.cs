using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Requests;

public sealed class ChangeOrderStatusRequest
{
    [Required]
    public string Status { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Notes { get; init; }

    public DateTimeOffset? Timestamp { get; init; }
}
