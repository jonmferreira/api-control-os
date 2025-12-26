using System.Linq;
using FluentValidation;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Api.Validators;

public sealed class PublishChecklistTemplateRequestValidator : AbstractValidator<PublishChecklistTemplateRequest>
{
    public PublishChecklistTemplateRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.PublishedBy)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Items)
            .NotEmpty()
            .WithMessage("At least one item must be provided.")
            .Must(HaveUniqueDisplayOrder)
            .WithMessage("DisplayOrder values must be unique.");

        RuleForEach(request => request.Items)
            .SetValidator(new PublishChecklistTemplateItemRequestValidator());
    }

    private static bool HaveUniqueDisplayOrder(IReadOnlyCollection<PublishChecklistTemplateItemRequest> items)
    {
        return items.Select(item => item.DisplayOrder).Distinct().Count() == items.Count;
    }
}

public sealed class PublishChecklistTemplateItemRequestValidator : AbstractValidator<PublishChecklistTemplateItemRequest>
{
    public PublishChecklistTemplateItemRequestValidator()
    {
        RuleFor(item => item.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(item => item.CustomInputComponentId)
            .NotNull()
            .When(item => item.HasCustomInput);

        RuleFor(item => item.DefaultOutcome)
            .Must(BeValidOutcome)
            .When(item => !string.IsNullOrWhiteSpace(item.DefaultOutcome))
            .WithMessage("DefaultOutcome must be Approved, Rejected or NotApplicable when provided.");

        RuleFor(item => item.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeValidOutcome(string? outcome)
    {
        return outcome is not null && Enum.TryParse(outcome, ignoreCase: true, out ChecklistOutcome _);
    }
}
