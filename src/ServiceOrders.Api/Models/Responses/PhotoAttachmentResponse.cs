namespace ServiceOrders.Api.Models.Responses;

public sealed class PhotoAttachmentResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid? OrderOfServiceId { get; set; }
    public Guid? ChecklistResponseItemId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
