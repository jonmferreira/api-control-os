namespace ServiceOrders.Api.Models.Responses;

public sealed class CustomInputComponentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string JsonBody { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
}
