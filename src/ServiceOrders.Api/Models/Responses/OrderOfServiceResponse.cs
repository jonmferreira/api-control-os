namespace ServiceOrders.Api.Models.Responses;

public sealed class OrderOfServiceResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? AssignedTechnician { get; set; }
    public DateTimeOffset OpenedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }
    public string? ClosingNotes { get; set; }
    public string Status { get; set; } = string.Empty;
    public ChecklistResponseResponse? ChecklistResponse { get; set; }
    public IReadOnlyCollection<PhotoAttachmentResponse> Attachments { get; set; } =
        Array.Empty<PhotoAttachmentResponse>();
}
