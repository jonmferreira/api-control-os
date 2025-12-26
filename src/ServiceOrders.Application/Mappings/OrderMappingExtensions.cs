using System.Linq;
using ServiceOrders.Application.Dtos;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Application.Mappings;

public static class OrderMappingExtensions
{
    public static OrderOfServiceDto ToDto(this OrderOfService order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new OrderOfServiceDto(
            order.Id,
            order.Title,
            order.Description,
            order.AssignedTechnician,
            order.OpenedAt,
            order.CompletedAt,
            order.RejectedAt,
            order.ClosingNotes,
            order.Status.ToString(),
            order.ChecklistResponse?.ToDto(),
            order.Attachments.Select(a => a.ToDto()).ToArray());
    }

    public static ChecklistTemplateDto ToDto(this ChecklistTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new ChecklistTemplateDto(
            template.Id,
            template.Title,
            template.PublishedBy,
            template.IsPublished,
            template.UpdatedAt,
            template.Items.Select(i => i.ToDto()).ToArray());
    }

    public static ChecklistTemplateItemDto ToDto(this ChecklistTemplateItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new ChecklistTemplateItemDto(
            item.Id,
            item.Description,
            item.HasCustomInput,
            item.CustomInputComponentId,
            item.DefaultOutcome?.ToString(),
            item.DisplayOrder);
    }

    public static CustomInputComponentDto ToDto(this CustomInputComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);

        return new CustomInputComponentDto(component.Id, component.Name, component.JsonBody, component.UpdatedAt);
    }

    public static ChecklistResponseDto ToDto(this ChecklistResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        return new ChecklistResponseDto(
            response.Id,
            response.ChecklistTemplateId,
            response.CreatedAt,
            response.Items.Select(i => i.ToDto()).ToArray());
    }

    public static ChecklistResponseItemDto ToDto(this ChecklistResponseItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new ChecklistResponseItemDto(
            item.Id,
            item.ChecklistTemplateItemId,
            item.Outcome.ToString(),
            item.CustomInputPayload,
            item.Observation,
            item.Attachments.Select(a => a.ToDto()).ToArray());
    }

    public static PhotoAttachmentDto ToDto(this PhotoAttachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        return new PhotoAttachmentDto(
            attachment.Id,
            attachment.Url,
            attachment.Type.ToString(),
            attachment.OrderOfServiceId,
            attachment.ChecklistResponseItemId,
            attachment.CreatedAt);
    }
}
