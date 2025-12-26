using System.Text.Json;
using FluentValidation;
using ServiceOrders.Api.Models.Requests;

namespace ServiceOrders.Api.Validators;

public sealed class CreateCustomInputComponentRequestValidator : AbstractValidator<CreateCustomInputComponentRequest>
{
    public CreateCustomInputComponentRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.JsonBody)
            .NotEmpty()
            .Must(BeValidJson)
            .WithMessage("JsonBody must contain a valid JSON payload.")
            .Must(ContainFieldsArray)
            .WithMessage("JsonBody must include a 'fields' array with required field definitions.");
    }

    private static bool BeValidJson(string json)
    {
        try
        {
            _ = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool ContainFieldsArray(string json)
    {
        try
        {
            using var document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("fields", out var fieldsElement) ||
                fieldsElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            return fieldsElement.EnumerateArray()
                .Any(field => field.TryGetProperty("name", out var nameProp) &&
                              nameProp.ValueKind == JsonValueKind.String &&
                              !string.IsNullOrWhiteSpace(nameProp.GetString()) &&
                              field.TryGetProperty("required", out var requiredProp) &&
                              requiredProp.ValueKind == JsonValueKind.True);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
