namespace ServiceOrders.Api.Models.Responses;

public sealed class ChecklistResponseResponse
{
    public Guid Id { get; set; }
    public Guid ChecklistTemplateId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyCollection<ChecklistResponseItemResponse> Items { get; set; } =
        Array.Empty<ChecklistResponseItemResponse>();
}

public sealed class ChecklistResponseItemResponse
{
    public Guid Id { get; set; }
    public Guid ChecklistTemplateItemId { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public string? CustomInputPayload { get; set; }
    public string? Observation { get; set; }
    public IReadOnlyCollection<PhotoAttachmentResponse> Attachments { get; set; } =
        Array.Empty<PhotoAttachmentResponse>();
}
