namespace ServiceOrders.Api.Models.Responses;

public sealed class ChecklistTemplateResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PublishedBy { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public IReadOnlyCollection<ChecklistTemplateItemResponse> Items { get; set; } =
        Array.Empty<ChecklistTemplateItemResponse>();
}

public sealed class ChecklistTemplateItemResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool HasCustomInput { get; set; }
    public Guid? CustomInputComponentId { get; set; }
    public string? DefaultOutcome { get; set; }
    public int DisplayOrder { get; set; }
}
