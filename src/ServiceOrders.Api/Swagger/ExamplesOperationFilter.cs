using System;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServiceOrders.Api.Swagger;

public sealed class ExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody?.Content is null)
        {
            return;
        }

        var path = context.ApiDescription.RelativePath ?? string.Empty;
        if (path.Contains("custom-components", StringComparison.OrdinalIgnoreCase)
            && operation.RequestBody.Content.TryGetValue("application/json", out var customComponentContent))
        {
            customComponentContent.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("DadosCarro"),
                ["jsonBody"] = new OpenApiString("{\n  \"fields\": [\n    { \"name\": \"placa\", \"label\": \"Placa\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"chassi\", \"label\": \"Chassi\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"modelo\", \"label\": \"Modelo\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"ano\", \"label\": \"Ano\", \"type\": \"number\", \"required\": true }\n  ]\n}")
            };
        }

        if (path.Contains("checklist-responses", StringComparison.OrdinalIgnoreCase)
            && string.Equals(context.ApiDescription.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)
            && operation.RequestBody.Content.TryGetValue("application/json", out var checklistResponseContent))
        {
            checklistResponseContent.Example = new OpenApiObject
            {
                ["orderOfServiceId"] = new OpenApiString("cdbbd9d1-8c7b-4d3f-a7a8-6a1db49f2bb8"),
                ["checklistTemplateId"] = new OpenApiString("55f21c67-991b-4a32-8d25-6c0fc63fd804"),
                ["items"] = new OpenApiArray
                {
                    new OpenApiObject
                    {
                        ["checklistTemplateItemId"] = new OpenApiString("6f6b46ce-92ff-4e2d-8ef5-b2787a026c2d"),
                        ["outcome"] = new OpenApiString("Approved"),
                        ["observation"] = new OpenApiString("Checklist preenchido no recebimento do veículo."),
                        ["photoUrls"] = new OpenApiArray()
                    },
                    new OpenApiObject
                    {
                        ["checklistTemplateItemId"] = new OpenApiString("12f8ab2d-21b2-4efc-9cf2-554e24e8fd0c"),
                        ["outcome"] = new OpenApiString("Rejected"),
                        ["customInputPayload"] = new OpenApiString("{ \"placa\": \"ABC1234\", \"chassi\": \"9BWZZZ377VT004251\", \"modelo\": \"Fusca\", \"ano\": 1974 }"),
                        ["observation"] = new OpenApiString("Foto obrigatória em caso de reprovação."),
                        ["photoUrls"] = new OpenApiArray
                        {
                            new OpenApiString("https://cdn.example.com/os/itens/12f8ab2d/foto-reprovacao.jpg")
                        }
                    }
                }
            };
        }
    }
}
