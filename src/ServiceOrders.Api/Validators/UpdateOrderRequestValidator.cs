using FluentValidation;
using ServiceOrders.Api.Models.Requests;

namespace ServiceOrders.Api.Validators;

public sealed class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(request => request.AssignedTechnician)
            .MaximumLength(200);
    }
}
