using ServiceOrders.Domain.Entities;

namespace ServiceOrders.UnitTests.Entities;

public class OrderOfServiceTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() => new OrderOfService(Guid.Empty, "Repair brakes", "Desc", null));
    }

    [Fact]
    public void Constructor_Should_Throw_When_Title_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() => new OrderOfService(Guid.NewGuid(), "  ", "Desc", null));
    }

    [Fact]
    public void ChangeStatus_Should_Require_Checklist_For_Completion()
    {
        var order = new OrderOfService(Guid.NewGuid(), "Install sensor", "Initial setup", "Alex", DateTimeOffset.Parse("2024-01-01T10:00:00Z"));

        Assert.Throws<InvalidOperationException>(() => order.ChangeStatus(OrderStatus.Completed, "done", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void AttachChecklistResponse_Should_Set_Status_Allows_Completion()
    {
        var order = new OrderOfService(Guid.NewGuid(), "Inspect site", "Check", "Maria");
        var response = new ChecklistResponse(Guid.NewGuid(), order.Id, Guid.NewGuid());

        order.AttachChecklistResponse(response);
        order.ChangeStatus(OrderStatus.Completed, "done", DateTimeOffset.Parse("2024-01-02T12:00:00Z"));

        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(DateTimeOffset.Parse("2024-01-02T12:00:00Z"), order.CompletedAt);
        Assert.Equal("done", order.ClosingNotes);
    }
}
