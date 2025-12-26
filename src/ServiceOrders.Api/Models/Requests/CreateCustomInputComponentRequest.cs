using System.ComponentModel.DataAnnotations;

namespace ServiceOrders.Api.Models.Requests;

public sealed class CreateCustomInputComponentRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public string JsonBody { get; init; } = string.Empty;
}
