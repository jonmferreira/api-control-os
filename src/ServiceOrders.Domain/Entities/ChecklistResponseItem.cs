using System.Collections.ObjectModel;

namespace ServiceOrders.Domain.Entities;

public sealed class ChecklistResponseItem
{
    private readonly List<PhotoAttachment> _attachments = new();

    private ChecklistResponseItem()
    {
        // EF Core
    }

    public ChecklistResponseItem(
        Guid id,
        Guid checklistResponseId,
        Guid checklistTemplateItemId,
        ChecklistOutcome outcome,
        string? customInputPayload,
        string? observation)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty.", nameof(id));
        }

        if (checklistResponseId == Guid.Empty)
        {
            throw new ArgumentException("ChecklistResponseId must not be empty.", nameof(checklistResponseId));
        }

        if (checklistTemplateItemId == Guid.Empty)
        {
            throw new ArgumentException("ChecklistTemplateItemId must not be empty.", nameof(checklistTemplateItemId));
        }

        Id = id;
        ChecklistResponseId = checklistResponseId;
        ChecklistTemplateItemId = checklistTemplateItemId;
        Outcome = outcome;
        CustomInputPayload = customInputPayload;
        Observation = observation?.Trim();
    }

    public Guid Id { get; private set; }

    public Guid ChecklistResponseId { get; private set; }

    public ChecklistResponse? ChecklistResponse { get; private set; }

    public Guid ChecklistTemplateItemId { get; private set; }

    public ChecklistTemplateItem? ChecklistTemplateItem { get; private set; }

    public ChecklistOutcome Outcome { get; private set; }

    public string? CustomInputPayload { get; private set; }

    public string? Observation { get; private set; }

    public IReadOnlyCollection<PhotoAttachment> Attachments => new ReadOnlyCollection<PhotoAttachment>(_attachments);

    public void AddAttachment(PhotoAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        if (attachment.Type != PhotoAttachmentType.ChecklistItemEvidence)
        {
            throw new InvalidOperationException("Attachment type must be ChecklistItemEvidence when linked to a checklist item.");
        }

        if (attachment.ChecklistResponseItemId != Id)
        {
            throw new InvalidOperationException("Attachment must reference this checklist response item.");
        }

        _attachments.Add(attachment);
    }
}
