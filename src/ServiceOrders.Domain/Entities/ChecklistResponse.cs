using System.Collections.ObjectModel;

namespace ServiceOrders.Domain.Entities;

public sealed class ChecklistResponse
{
    private readonly List<ChecklistResponseItem> _items = new();

    private ChecklistResponse()
    {
        // EF Core
    }

    public ChecklistResponse(Guid id, Guid orderOfServiceId, Guid checklistTemplateId, DateTimeOffset? createdAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (orderOfServiceId == Guid.Empty)
        {
            throw new ArgumentException("OrderOfServiceId must not be empty.", nameof(orderOfServiceId));
        }

        if (checklistTemplateId == Guid.Empty)
        {
            throw new ArgumentException("ChecklistTemplateId must not be empty.", nameof(checklistTemplateId));
        }

        Id = id;
        OrderOfServiceId = orderOfServiceId;
        ChecklistTemplateId = checklistTemplateId;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrderOfServiceId { get; private set; }

    public OrderOfService? OrderOfService { get; private set; }

    public Guid ChecklistTemplateId { get; private set; }

    public ChecklistTemplate? ChecklistTemplate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<ChecklistResponseItem> Items => new ReadOnlyCollection<ChecklistResponseItem>(_items);

    public void AddItem(ChecklistResponseItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.ChecklistTemplateItemId == Guid.Empty)
        {
            throw new ArgumentException("ChecklistTemplateItemId must be provided.", nameof(item));
        }

        _items.Add(item);
    }
}
