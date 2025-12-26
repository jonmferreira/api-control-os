namespace ServiceOrders.Domain.Entities;

public sealed class PhotoAttachment
{
    private PhotoAttachment()
    {
        // EF Core
    }

    public PhotoAttachment(Guid id, string url, PhotoAttachmentType type, Guid? orderOfServiceId, Guid? checklistResponseItemId, DateTimeOffset? createdAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Url must not be empty.", nameof(url));
        }

        if (type == PhotoAttachmentType.OrderEvidence && orderOfServiceId is null)
        {
            throw new ArgumentException("OrderOfServiceId is required when the attachment targets an order.", nameof(orderOfServiceId));
        }

        if (type == PhotoAttachmentType.ChecklistItemEvidence && checklistResponseItemId is null)
        {
            throw new ArgumentException("ChecklistResponseItemId is required when the attachment targets a checklist item.", nameof(checklistResponseItemId));
        }

        Id = id;
        Url = url.Trim();
        Type = type;
        OrderOfServiceId = orderOfServiceId;
        ChecklistResponseItemId = checklistResponseItemId;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Url { get; private set; } = string.Empty;

    public PhotoAttachmentType Type { get; private set; }

    public Guid? OrderOfServiceId { get; private set; }

    public OrderOfService? OrderOfService { get; private set; }

    public Guid? ChecklistResponseItemId { get; private set; }

    public ChecklistResponseItem? ChecklistResponseItem { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
}
