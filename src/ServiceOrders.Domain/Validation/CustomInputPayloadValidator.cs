using System.Text.Json;
using ServiceOrders.Domain.Entities;

namespace ServiceOrders.Domain.Validation;

public static class CustomInputPayloadValidator
{
    public static bool TryValidate(CustomInputComponent component, string payload, out string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(component);

        if (string.IsNullOrWhiteSpace(payload))
        {
            errorMessage = "CustomInputPayload must not be empty for items that require custom input.";
            return false;
        }

        if (!TryGetRequiredFields(component, out var requiredFields, out errorMessage))
        {
            return false;
        }

        try
        {
            using var payloadDocument = JsonDocument.Parse(payload);

            if (payloadDocument.RootElement.ValueKind != JsonValueKind.Object)
            {
                errorMessage = "CustomInputPayload must be a JSON object.";
                return false;
            }

            foreach (var requiredField in requiredFields)
            {
                if (!payloadDocument.RootElement.TryGetProperty(requiredField, out var value))
                {
                    errorMessage = $"CustomInputPayload is missing required field '{requiredField}'.";
                    return false;
                }

                if (value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                {
                    errorMessage = $"CustomInputPayload field '{requiredField}' cannot be null.";
                    return false;
                }

                if (value.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(value.GetString()))
                {
                    errorMessage = $"CustomInputPayload field '{requiredField}' cannot be empty.";
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }
        catch (JsonException)
        {
            errorMessage = "CustomInputPayload must contain valid JSON.";
            return false;
        }
    }

    public static bool TryGetRequiredFields(CustomInputComponent component, out IReadOnlyCollection<string> requiredFields, out string errorMessage)
    {
        try
        {
            using var document = JsonDocument.Parse(component.JsonBody);

            if (!document.RootElement.TryGetProperty("fields", out var fieldsElement) ||
                fieldsElement.ValueKind != JsonValueKind.Array)
            {
                errorMessage = "JsonBody must define a 'fields' array describing the component schema.";
                requiredFields = Array.Empty<string>();
                return false;
            }

            var fields = new List<string>();

            foreach (var field in fieldsElement.EnumerateArray())
            {
                if (!field.TryGetProperty("name", out var nameProp) || nameProp.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var name = nameProp.GetString();
                var isRequired = field.TryGetProperty("required", out var requiredProp) &&
                                 requiredProp.ValueKind == JsonValueKind.True;

                if (isRequired && !string.IsNullOrWhiteSpace(name))
                {
                    fields.Add(name);
                }
            }

            if (fields.Count == 0)
            {
                errorMessage = "JsonBody must define at least one required field.";
                requiredFields = Array.Empty<string>();
                return false;
            }

            requiredFields = fields;
            errorMessage = string.Empty;
            return true;
        }
        catch (JsonException)
        {
            errorMessage = "JsonBody must be a valid JSON document.";
            requiredFields = Array.Empty<string>();
            return false;
        }
    }
}
