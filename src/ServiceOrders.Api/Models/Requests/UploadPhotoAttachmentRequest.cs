using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Requests;

public sealed class UploadPhotoAttachmentRequest
{
    [Required]
    [MaxLength(500)]
    public string Url { get; init; } = string.Empty;

    [Required]
    public string Type { get; init; } = string.Empty;

    public Guid? OrderOfServiceId { get; init; }

    public Guid? ChecklistResponseItemId { get; init; }
}
