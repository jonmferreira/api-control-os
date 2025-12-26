using System.Collections.ObjectModel;

namespace ServiceOrders.Domain.Entities;

public sealed class OrderOfService
{
    private readonly List<PhotoAttachment> _attachments = new();

    private OrderOfService()
    {
        // EF Core
    }

    public OrderOfService(Guid id, string title, string description, string? assignedTechnician = null, DateTimeOffset? openedAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be empty.", nameof(description));
        }

        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        AssignedTechnician = assignedTechnician?.Trim();
        OpenedAt = openedAt ?? DateTimeOffset.UtcNow;
        Status = OrderStatus.Open;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string? AssignedTechnician { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTimeOffset OpenedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? RejectedAt { get; private set; }

    public string? ClosingNotes { get; private set; }

    public Guid? ChecklistResponseId { get; private set; }

    public ChecklistResponse? ChecklistResponse { get; private set; }

    public IReadOnlyCollection<PhotoAttachment> Attachments => new ReadOnlyCollection<PhotoAttachment>(_attachments);

    public void UpdateDetails(string title, string description, string? assignedTechnician)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be empty.", nameof(description));
        }

        Title = title.Trim();
        Description = description.Trim();
        AssignedTechnician = assignedTechnician?.Trim();
    }

    public void AttachChecklistResponse(ChecklistResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.OrderOfServiceId != Id)
        {
            throw new InvalidOperationException("Checklist response must belong to the same order of service.");
        }

        ChecklistResponse = response;
        ChecklistResponseId = response.Id;
    }

    public void ChangeStatus(OrderStatus status, string? notes = null, DateTimeOffset? timestamp = null)
    {
        if (Status == status)
        {
            return;
        }

        if (Status is OrderStatus.Completed or OrderStatus.Rejected)
        {
            throw new InvalidOperationException("Completed or rejected orders cannot change status.");
        }

        if (status == OrderStatus.Completed && ChecklistResponse is null)
        {
            throw new InvalidOperationException("Cannot complete an order without a checklist response.");
        }

        Status = status;
        ClosingNotes = notes?.Trim();

        if (status == OrderStatus.Completed)
        {
            CompletedAt = timestamp ?? DateTimeOffset.UtcNow;
        }

        if (status == OrderStatus.Rejected)
        {
            RejectedAt = timestamp ?? DateTimeOffset.UtcNow;
        }
    }

    public void AddAttachment(PhotoAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        if (attachment.Type != PhotoAttachmentType.OrderEvidence)
        {
            throw new InvalidOperationException("Attachment type must be OrderEvidence when linked directly to the order.");
        }

        if (attachment.OrderOfServiceId != Id)
        {
            throw new InvalidOperationException("Attachment must reference this order of service.");
        }

        _attachments.Add(attachment);
    }
}
