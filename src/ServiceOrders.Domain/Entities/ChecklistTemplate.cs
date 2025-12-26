using System.Collections.ObjectModel;

namespace ServiceOrders.Domain.Entities;

public sealed class ChecklistTemplate
{
    private readonly List<ChecklistTemplateItem> _items = new();

    private ChecklistTemplate()
    {
        // EF Core
    }

    public ChecklistTemplate(Guid id, string title, string publishedBy, bool isPublished = false, DateTimeOffset? updatedAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title must not be empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(publishedBy))
        {
            throw new ArgumentException("PublishedBy must not be empty.", nameof(publishedBy));
        }

        Id = id;
        Title = title.Trim();
        PublishedBy = publishedBy.Trim();
        IsPublished = isPublished;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string PublishedBy { get; private set; } = string.Empty;

    public bool IsPublished { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyCollection<ChecklistTemplateItem> Items => new ReadOnlyCollection<ChecklistTemplateItem>(_items);

    public void ReplaceItems(IEnumerable<ChecklistTemplateItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        _items.Clear();
        _items.AddRange(items);
    }

    public void Publish()
    {
        IsPublished = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
