namespace ServiceOrders.Domain.Entities;

public sealed class ChecklistTemplateItem
{
    private ChecklistTemplateItem()
    {
        // EF Core
    }

    public ChecklistTemplateItem(
        Guid id,
        Guid checklistTemplateId,
        string description,
        bool hasCustomInput,
        Guid? customInputComponentId,
        ChecklistOutcome? defaultOutcome,
        int displayOrder)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (checklistTemplateId == Guid.Empty)
        {
            throw new ArgumentException("Checklist template id must not be empty.", nameof(checklistTemplateId));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be empty.", nameof(description));
        }

        if (hasCustomInput && customInputComponentId is null)
        {
            throw new ArgumentException("CustomInputComponentId is required when HasCustomInput is true.", nameof(customInputComponentId));
        }

        Id = id;
        ChecklistTemplateId = checklistTemplateId;
        Description = description.Trim();
        HasCustomInput = hasCustomInput;
        CustomInputComponentId = customInputComponentId;
        DefaultOutcome = defaultOutcome;
        DisplayOrder = displayOrder;
    }

    public Guid Id { get; private set; }

    public Guid ChecklistTemplateId { get; private set; }

    public ChecklistTemplate? ChecklistTemplate { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public bool HasCustomInput { get; private set; }

    public Guid? CustomInputComponentId { get; private set; }

    public CustomInputComponent? CustomInputComponent { get; private set; }

    public ChecklistOutcome? DefaultOutcome { get; private set; }

    public int DisplayOrder { get; private set; }
}
