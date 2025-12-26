using FluentValidation;
using ServiceOrders.Api.Models.Requests;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Api.Validators;

public sealed class ChangeOrderStatusRequestValidator : AbstractValidator<ChangeOrderStatusRequest>
{
    public ChangeOrderStatusRequestValidator()
    {
        RuleFor(request => request.Status)
            .NotEmpty()
            .Must(BeValidStatus)
            .WithMessage("Status must be one of: Open, InProgress, Completed, Rejected.");

        RuleFor(request => request.Notes)
            .MaximumLength(2000);
    }

    private static bool BeValidStatus(string status)
    {
        return Enum.TryParse(status, ignoreCase: true, out OrderStatus _);
    }
}
