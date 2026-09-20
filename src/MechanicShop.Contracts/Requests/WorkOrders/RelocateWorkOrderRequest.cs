using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public record RelocateWorkOrderRequest
{
    public DateTimeOffset NewStartAtUtc { get; set; }
    public Spots NewSpot { get; set; }
}

