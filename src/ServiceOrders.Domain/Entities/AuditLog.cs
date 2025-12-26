namespace ServiceOrders.Domain.Entities;

public sealed class AuditLog
{
    private AuditLog()
    {
        // EF Core
    }

    public AuditLog(Guid id, string action, string resourceType, Guid resourceId, string performedBy, string details, DateTimeOffset? occurredAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            throw new ArgumentException("Action must not be empty.", nameof(action));
        }

        if (string.IsNullOrWhiteSpace(resourceType))
        {
            throw new ArgumentException("Resource type must not be empty.", nameof(resourceType));
        }

        if (resourceId == Guid.Empty)
        {
            throw new ArgumentException("Resource id must not be empty.", nameof(resourceId));
        }

        if (string.IsNullOrWhiteSpace(performedBy))
        {
            throw new ArgumentException("PerformedBy must not be empty.", nameof(performedBy));
        }

        if (string.IsNullOrWhiteSpace(details))
        {
            throw new ArgumentException("Details must not be empty.", nameof(details));
        }

        Id = id;
        Action = action.Trim();
        ResourceType = resourceType.Trim();
        ResourceId = resourceId;
        PerformedBy = performedBy.Trim();
        Details = details.Trim();
        OccurredAt = occurredAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string ResourceType { get; private set; } = string.Empty;

    public Guid ResourceId { get; private set; }

    public string PerformedBy { get; private set; } = string.Empty;

    public string Details { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }
}
