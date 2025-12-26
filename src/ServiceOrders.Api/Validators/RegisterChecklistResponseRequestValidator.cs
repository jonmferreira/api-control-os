using System;
using System.Linq;
using FluentValidation;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Domain.Entities;
using ServiceOrders.Domain.Repositories;
using ServiceOrders.Domain.Validation;

namespace ServiceOrders.Api.Validators;

public sealed class RegisterChecklistResponseRequestValidator : AbstractValidator<RegisterChecklistResponseRequest>
{
    private readonly IChecklistTemplateRepository _templates;
    private readonly ICustomInputComponentRepository _components;

    public RegisterChecklistResponseRequestValidator(
        IChecklistTemplateRepository templates,
        ICustomInputComponentRepository components)
    {
        _templates = templates ?? throw new ArgumentNullException(nameof(templates));
        _components = components ?? throw new ArgumentNullException(nameof(components));

        RuleFor(request => request.OrderOfServiceId)
            .NotEmpty();

        RuleFor(request => request.ChecklistTemplateId)
            .NotEmpty();

        RuleFor(request => request.Items)
            .NotEmpty()
            .WithMessage("At least one checklist item response is required.");

        RuleForEach(request => request.Items)
            .SetValidator(new RegisterChecklistResponseItemRequestValidator());

        RuleFor(request => request)
            .CustomAsync(ValidateAgainstTemplateAsync);
    }

    private async Task ValidateAgainstTemplateAsync(
        RegisterChecklistResponseRequest request,
        ValidationContext<RegisterChecklistResponseRequest> context,
        CancellationToken cancellationToken)
    {
        var template = await _templates.GetByIdWithItemsAsync(request.ChecklistTemplateId, cancellationToken);
        if (template is null)
        {
            context.AddFailure(nameof(request.ChecklistTemplateId), "Checklist template not found.");
            return;
        }

        var templateItems = template.Items.ToDictionary(item => item.Id);
        var items = request.Items ?? Array.Empty<RegisterChecklistResponseItemRequest>();

        for (var index = 0; index < items.Count; index++)
        {
            var item = items.ElementAt(index);
            if (!templateItems.TryGetValue(item.ChecklistTemplateItemId, out var templateItem))
            {
                context.AddFailure($"Items[{index}].ChecklistTemplateItemId", $"Item {item.ChecklistTemplateItemId} does not belong to template {template.Id}.");
                continue;
            }

            if (templateItem.HasCustomInput)
            {
                if (templateItem.CustomInputComponentId is null)
                {
                    context.AddFailure($"Items[{index}].ChecklistTemplateItemId", "Template item is missing CustomInputComponentId.");
                    continue;
                }

                var component = await _components.GetByIdAsync(templateItem.CustomInputComponentId.Value, cancellationToken);
                if (component is null)
                {
                    context.AddFailure($"Items[{index}].ChecklistTemplateItemId", $"Custom input component {templateItem.CustomInputComponentId} not found.");
                    continue;
                }

                if (!CustomInputPayloadValidator.TryValidate(component, item.CustomInputPayload ?? string.Empty, out var payloadError))
                {
                    context.AddFailure($"Items[{index}].CustomInputPayload", payloadError);
                }
            }
            else if (!string.IsNullOrWhiteSpace(item.CustomInputPayload))
            {
                context.AddFailure($"Items[{index}].CustomInputPayload", "CustomInputPayload should not be provided when the template item does not require custom input.");
            }
        }
    }
}

public sealed class RegisterChecklistResponseItemRequestValidator : AbstractValidator<RegisterChecklistResponseItemRequest>
{
    public RegisterChecklistResponseItemRequestValidator()
    {
        RuleFor(item => item.ChecklistTemplateItemId)
            .NotEmpty();

        RuleFor(item => item.Outcome)
            .NotEmpty()
            .Must(BeValidOutcome)
            .WithMessage("Outcome must be Approved, Rejected or NotApplicable.");

        RuleFor(item => item.CustomInputPayload)
            .MaximumLength(4000);

        RuleFor(item => item.Observation)
            .MaximumLength(1000);

        RuleFor(item => item.PhotoUrls)
            .NotEmpty()
            .When(item => IsRejected(item.Outcome))
            .WithMessage("Rejected items must provide at least one photo.");

        RuleForEach(item => item.PhotoUrls)
            .NotEmpty()
            .MaximumLength(500);
    }

    private static bool BeValidOutcome(string outcome)
    {
        return Enum.TryParse(outcome, ignoreCase: true, out ChecklistOutcome _);
    }

    private static bool IsRejected(string outcome)
    {
        return Enum.TryParse(outcome, ignoreCase: true, out ChecklistOutcome parsed) && parsed == ChecklistOutcome.Rejected;
    }
}
