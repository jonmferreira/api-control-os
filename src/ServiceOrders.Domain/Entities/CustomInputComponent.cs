namespace ServiceOrders.Domain.Entities;

public sealed class CustomInputComponent
{
    private CustomInputComponent()
    {
        // EF Core
    }

    public CustomInputComponent(Guid id, string name, string jsonBody, DateTimeOffset? updatedAt = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must not be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(jsonBody))
        {
            throw new ArgumentException("Json body must not be empty.", nameof(jsonBody));
        }

        Id = id;
        Name = name.Trim();
        JsonBody = jsonBody;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string JsonBody { get; private set; } = string.Empty;

    public DateTimeOffset UpdatedAt { get; private set; }
}
