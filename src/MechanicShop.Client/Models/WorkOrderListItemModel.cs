using MechanicShop.Contracts.Common;

namespace MechanicShop.Client.Models;


public class WorkOrderListItemModel
{
    public Guid WorkOrderId { get; set; }
    public Guid? InvoiceId { get; set; }
    public VehicleModel Vehicle { get; set; } = default!;
    public string? Customer { get; set; }
    public string? Labor { get; set; }
    public WorkOrderStatus State { get; set; }
    public Spots Spots { get; set; }
    public DateTimeOffset StartAtUtc { get; set; }
    public DateTimeOffset EndAtUtc { get; set; }
    public List<string> RepairTasks { get; set; } = [];
}
