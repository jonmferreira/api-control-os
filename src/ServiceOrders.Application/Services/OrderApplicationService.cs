using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using ServiceOrders.Application.Abstractions;
using ServiceOrders.Application.Commands;
using ServiceOrders.Application.Dtos;
using ServiceOrders.Application.Mappings;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Domain.Validation;

namespace ServiceOrders.Application.Services;

public sealed class OrderApplicationService : IOrderApplicationService
{
    private readonly IOrderOfServiceRepository _orders;
    private readonly IChecklistTemplateRepository _templates;
    private readonly IChecklistResponseRepository _responses;
    private readonly ICustomInputComponentRepository _components;
    private readonly IPhotoAttachmentRepository _attachments;
    private readonly IAuditLogRepository _auditLogs;
    private readonly ILogger<OrderApplicationService> _logger;

    public OrderApplicationService(
        IOrderOfServiceRepository orders,
        IChecklistTemplateRepository templates,
        IChecklistResponseRepository responses,
        ICustomInputComponentRepository components,
        IPhotoAttachmentRepository attachments,
        IAuditLogRepository auditLogs,
        ILogger<OrderApplicationService> logger)
    {
        _orders = orders ?? throw new ArgumentNullException(nameof(orders));
        _templates = templates ?? throw new ArgumentNullException(nameof(templates));
        _responses = responses ?? throw new ArgumentNullException(nameof(responses));
        _components = components ?? throw new ArgumentNullException(nameof(components));
        _attachments = attachments ?? throw new ArgumentNullException(nameof(attachments));
        _auditLogs = auditLogs ?? throw new ArgumentNullException(nameof(auditLogs));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrderOfServiceDto> CreateAsync(CreateOrderOfServiceCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            throw new ArgumentException("Title is required.", nameof(command.Title));
        }

        if (string.IsNullOrWhiteSpace(command.Description))
        {
            throw new ArgumentException("Description is required.", nameof(command.Description));
        }

        var order = new OrderOfService(Guid.NewGuid(), command.Title, command.Description, command.AssignedTechnician, command.OpenedAt);
        await _orders.AddAsync(order, cancellationToken);

        return order.ToDto();
    }

    public async Task<OrderOfServiceDto> UpdateAsync(UpdateOrderOfServiceCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var order = await _orders.GetByIdWithDetailsAsync(command.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {command.OrderId} not found.");

        order.UpdateDetails(command.Title, command.Description, command.AssignedTechnician);
        await _orders.UpdateAsync(order, cancellationToken);

        return order.ToDto();
    }

    public async Task<OrderOfServiceDto> ChangeStatusAsync(ChangeOrderStatusCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var order = await _orders.GetByIdWithDetailsAsync(command.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {command.OrderId} not found.");

        var previousStatus = order.Status;
        var timestamp = command.Timestamp ?? DateTimeOffset.UtcNow;

        order.ChangeStatus(command.Status, command.Notes, timestamp);
        await _orders.UpdateAsync(order, cancellationToken);

        var auditLog = new AuditLog(
            Guid.NewGuid(),
            "OrderStatusChanged",
            nameof(OrderOfService),
            order.Id,
            string.IsNullOrWhiteSpace(command.ChangedBy) ? "system" : command.ChangedBy!,
            $"Status changed from {previousStatus} to {command.Status} with notes '{command.Notes ?? "n/a"}' at {timestamp:O}.",
            timestamp);

        await _auditLogs.AddAsync(auditLog, cancellationToken);
        _logger.LogInformation("Recorded audit entry for status change of order {OrderId}", order.Id);

        return order.ToDto();
    }

    public async Task<OrderOfServiceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdWithDetailsAsync(id, cancellationToken);
        return order?.ToDto();
    }

    public async Task<IReadOnlyCollection<OrderOfServiceDto>> GetAsync(OrderStatus? status, string? technician, CancellationToken cancellationToken = default)
    {
        var orders = await _orders.GetByStatusAsync(status, technician, cancellationToken);
        return orders.Select(o => o.ToDto()).ToArray();
    }

    public async Task<ChecklistTemplateDto> PublishTemplateAsync(PublishChecklistTemplateCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            throw new ArgumentException("Template title is required.", nameof(command.Title));
        }

        if (string.IsNullOrWhiteSpace(command.PublishedBy))
        {
            throw new ArgumentException("PublishedBy is required.", nameof(command.PublishedBy));
        }

        var templateId = Guid.NewGuid();
        var template = new ChecklistTemplate(templateId, command.Title, command.PublishedBy, command.PublishImmediately);

        var items = command.Items
            .OrderBy(i => i.DisplayOrder)
            .Select(item => new ChecklistTemplateItem(
                Guid.NewGuid(),
                templateId,
                item.Description,
                item.HasCustomInput,
                item.CustomInputComponentId,
                item.DefaultOutcome,
                item.DisplayOrder))
            .ToArray();

        template.ReplaceItems(items);

        if (command.PublishImmediately)
        {
            template.Publish();
        }

        await _templates.AddAsync(template, cancellationToken);
        var auditLog = new AuditLog(
            Guid.NewGuid(),
            "ChecklistTemplatePublished",
            nameof(ChecklistTemplate),
            template.Id,
            command.PublishedBy,
            $"Template '{command.Title}' published (immediate: {command.PublishImmediately}).",
            DateTimeOffset.UtcNow);

        await _auditLogs.AddAsync(auditLog, cancellationToken);
        _logger.LogInformation("Recorded audit entry for checklist template {TemplateId}", template.Id);
        return template.ToDto();
    }

    public async Task<IReadOnlyCollection<ChecklistTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var templates = await _templates.GetAllAsync(cancellationToken);
        return templates.Select(t => t.ToDto()).ToArray();
    }

    public async Task<ChecklistTemplateDto?> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _templates.GetByIdWithItemsAsync(id, cancellationToken);
        return template?.ToDto();
    }

    public async Task<ChecklistResponseDto> RegisterChecklistResponseAsync(RegisterChecklistResponseCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var order = await _orders.GetByIdWithDetailsAsync(command.OrderOfServiceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Order {command.OrderOfServiceId} not found.");

        if (order.Status is OrderStatus.Completed or OrderStatus.Rejected)
        {
            throw new InvalidOperationException("Responses cannot be registered for finalized orders.");
        }

        if (order.ChecklistResponse is not null)
        {
            throw new InvalidOperationException("A checklist response has already been registered for this order.");
        }

        var template = await _templates.GetByIdWithItemsAsync(command.ChecklistTemplateId, cancellationToken)
            ?? throw new KeyNotFoundException($"Checklist template {command.ChecklistTemplateId} not found.");

        var templateItems = template.Items.ToDictionary(i => i.Id, i => i);

        var response = new ChecklistResponse(Guid.NewGuid(), order.Id, template.Id);

        foreach (var itemCommand in command.Items)
        {
            if (!templateItems.TryGetValue(itemCommand.ChecklistTemplateItemId, out var templateItem))
            {
                throw new ArgumentException($"Template item {itemCommand.ChecklistTemplateItemId} does not belong to template {template.Id}.");
            }

            if (templateItem.HasCustomInput && string.IsNullOrWhiteSpace(itemCommand.CustomInputPayload))
            {
                throw new ArgumentException($"Custom input payload is required for item {templateItem.Id}.");
            }

            if (!templateItem.HasCustomInput && !string.IsNullOrWhiteSpace(itemCommand.CustomInputPayload))
            {
                throw new ArgumentException($"Item {templateItem.Id} does not accept custom input.");
            }

            if (templateItem.HasCustomInput)
            {
                if (templateItem.CustomInputComponentId is null)
                {
                    throw new ArgumentException($"Template item {templateItem.Id} does not reference a custom component.");
                }

                var component = await _components.GetByIdAsync(templateItem.CustomInputComponentId.Value, cancellationToken)
                               ?? throw new InvalidOperationException($"Custom input component {templateItem.CustomInputComponentId} not found.");

                if (!CustomInputPayloadValidator.TryValidate(component, itemCommand.CustomInputPayload ?? string.Empty, out var payloadError))
                {
                    throw new ArgumentException(payloadError);
                }
            }

            if (itemCommand.Outcome == ChecklistOutcome.Rejected && (itemCommand.PhotoUrls is null || !itemCommand.PhotoUrls.Any()))
            {
                throw new ArgumentException($"Rejected item {templateItem.Id} must contain at least one photo.");
            }

            var responseItem = new ChecklistResponseItem(
                Guid.NewGuid(),
                response.Id,
                templateItem.Id,
                itemCommand.Outcome,
                itemCommand.CustomInputPayload,
                itemCommand.Observation);

            foreach (var url in itemCommand.PhotoUrls ?? Enumerable.Empty<string>())
            {
                var attachment = new PhotoAttachment(
                    Guid.NewGuid(),
                    url,
                    PhotoAttachmentType.ChecklistItemEvidence,
                    null,
                    responseItem.Id);
                responseItem.AddAttachment(attachment);
            }

            response.AddItem(responseItem);
        }

        order.AttachChecklistResponse(response);

        await _responses.AddAsync(response, cancellationToken);
        await _orders.UpdateAsync(order, cancellationToken);

        return response.ToDto();
    }

    public async Task<CustomInputComponentDto> CreateComponentAsync(CreateCustomInputComponentCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var component = new CustomInputComponent(Guid.NewGuid(), command.Name, command.JsonBody);
        await _components.AddAsync(component, cancellationToken);

        return component.ToDto();
    }

    public async Task<IReadOnlyCollection<CustomInputComponentDto>> GetComponentsAsync(CancellationToken cancellationToken = default)
    {
        var components = await _components.GetAllAsync(cancellationToken);
        return components.Select(c => c.ToDto()).ToArray();
    }

    public async Task<PhotoAttachmentDto> UploadAttachmentAsync(UploadPhotoAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Url))
        {
            throw new ArgumentException("Url is required.", nameof(command.Url));
        }

        if (!HasAllowedImageExtension(command.Url))
        {
            throw new ArgumentException("Url must reference an image (.jpg, .jpeg, .png, .gif, .heic or .webp).", nameof(command.Url));
        }

        PhotoAttachment attachment;

        if (command.Type == PhotoAttachmentType.OrderEvidence)
        {
            var order = await _orders.GetByIdWithDetailsAsync(command.OrderOfServiceId ?? Guid.Empty, cancellationToken)
                ?? throw new ArgumentException("OrderOfServiceId is required for order evidence uploads.", nameof(command.OrderOfServiceId));

            attachment = new PhotoAttachment(Guid.NewGuid(), command.Url, command.Type, order.Id, null);
            order.AddAttachment(attachment);
            await _orders.UpdateAsync(order, cancellationToken);
        }
        else
        {
            var item = await _responses.GetItemByIdAsync(command.ChecklistResponseItemId ?? Guid.Empty, cancellationToken)
                ?? throw new ArgumentException("ChecklistResponseItemId is required for checklist item evidence uploads.", nameof(command.ChecklistResponseItemId));

            attachment = new PhotoAttachment(Guid.NewGuid(), command.Url, command.Type, null, item.Id);
            item.AddAttachment(attachment);
            await _attachments.AddAsync(attachment, cancellationToken);
        }

        return attachment.ToDto();
    }

    private static bool HasAllowedImageExtension(string url)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".webp" };
        var path = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)
            ? (uri.IsAbsoluteUri ? uri.LocalPath : uri.OriginalString)
            : url;

        var extension = Path.GetExtension(path).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}
